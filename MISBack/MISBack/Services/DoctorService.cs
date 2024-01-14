using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;
using AutoMapper;
using BlogApi.Configurations;
using BlogApi.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MISBack.Data;
using MISBack.Data.Entities;
using MISBack.Data.Enums;
using MISBack.Data.Models;
using MISBack.Services.Interfaces;

namespace MISBack.Services;

public class DoctorService : IDoctorService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public DoctorService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<TokenResponseModel> RegisterDoctor(DoctorRegisterModel doctorRegisterModel)
    {
        doctorRegisterModel.Email = doctorRegisterModel.Email.ToLower().TrimEnd();
        doctorRegisterModel.Password = doctorRegisterModel.Password.TrimEnd();
        CheckPassword(doctorRegisterModel.Password);

        await UniqueCheck(doctorRegisterModel);
        await SpecialitiesCheck(doctorRegisterModel.Speciality);

        var salt = BCrypt.Net.BCrypt.GenerateSalt();
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(doctorRegisterModel.Password, salt);

        CheckGender(doctorRegisterModel.Gender);
        CheckBirthDate(doctorRegisterModel.BirthDate);

        await _context.Doctor.AddAsync(new Doctor
        {
            Id = Guid.NewGuid(),
            Name = doctorRegisterModel.Name,
            BirthDate = doctorRegisterModel.BirthDate,
            Email = doctorRegisterModel.Email,
            Gender = doctorRegisterModel.Gender,
            Password = hashedPassword,
            Phone = doctorRegisterModel.Phone,
            CreateTime = DateTime.UtcNow,
            Speciality = doctorRegisterModel.Speciality
        });
        await _context.SaveChangesAsync();

        var credentials = new LoginCredentialsModel
        {
            Email = doctorRegisterModel.Email,
            Password = doctorRegisterModel.Password
        };

        return await Login(credentials);
    }

    public async Task<TokenResponseModel> Login(LoginCredentialsModel credentials)
    {
        credentials.Email = credentials.Email.ToLower().TrimEnd();
        credentials.Password = credentials.Password.TrimEnd();

        var identity = await GetIdentity(credentials.Email, credentials.Password);

        var now = DateTime.UtcNow;

        var jwt = new JwtSecurityToken(
            issuer: AuthConfiguration.Issuer,
            audience: AuthConfiguration.Audience,
            notBefore: now,
            claims: identity.Claims,
            expires: now.AddMinutes(AuthConfiguration.Lifetime),
            signingCredentials: new SigningCredentials(AuthConfiguration.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256));

        var encodeJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        var result = new TokenResponseModel()
        {
            Token = encodeJwt
        };

        return result;
    }

    public async Task Logout(string token)
    {
        var alreadyExistsToken = await _context.Token.FirstOrDefaultAsync(x => x.InvalidToken == token);

        if (alreadyExistsToken == null)
        {
            var handler = new JwtSecurityTokenHandler();
            var expiredDate = handler.ReadJwtToken(token).ValidTo;
            _context.Token.Add(new Data.Entities.Token { InvalidToken = token, ExpiredDate = expiredDate });
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new UnauthorizedAccessException("Token is already invalid");
        }
    }

    public async Task<DoctorModel> GetDoctorProfile(Guid doctorId)
    {
        var doctorEntity = await _context.Doctor
            .FirstOrDefaultAsync(x => x.Id == doctorId);

        if (doctorEntity != null)
            return _mapper.Map<DoctorModel>(doctorEntity);
        
        throw new UnauthorizedAccessException("User not exists");
    }

    public async Task EditDoctorProfile(Guid doctorId, DoctorEditModel doctorEditModel)
    {
        var doctorEntity = await _context.Doctor
            .FirstOrDefaultAsync(x => x.Id == doctorId);

        if (doctorEntity == null)
        {
            throw new UnauthorizedAccessException("User not exists");
        }

        var doctor = await _context.Doctor
            .Where(d => d.Email == doctorEditModel.Email)
            .FirstOrDefaultAsync();

        if (doctor != null && doctor.Id != doctorId)
        {
            throw new ConflictException("Doctor with this Email already exists");
        }
        
        if(doctorEditModel.Phone != null)
        {
            doctor = await _context.Doctor
                .Where(d => d.Phone == doctorEditModel.Phone)
                .FirstOrDefaultAsync();

            if (doctor != null)
            {
                throw new ConflictException("Doctor with this Phone number already exists");
            }
        }
        
        CheckGender(doctorEditModel.Gender);
        CheckBirthDate(doctorEditModel.BirthDate);

        doctorEntity.Email = doctorEditModel.Email;
        doctorEntity.Name = doctorEditModel.Name;
        doctorEntity.BirthDate = doctorEditModel.BirthDate;
        doctorEntity.Gender = doctorEditModel.Gender;
        doctorEntity.Phone = doctorEditModel.Phone;

        await _context.SaveChangesAsync();
    }

    private async Task UniqueCheck(DoctorRegisterModel doctorRegisterModel)
    {
        var doctor = await _context.Doctor
            .Where(d => d.Email == doctorRegisterModel.Email)
            .FirstOrDefaultAsync();

        if (doctor != null)
        {
            throw new ConflictException("Doctor with this Email already exists");
        }
        
        if(doctorRegisterModel.Phone != null)
        {
            doctor = await _context.Doctor
                .Where(d => d.Phone == doctorRegisterModel.Phone)
                .FirstOrDefaultAsync();

            if (doctor != null)
            {
                throw new ConflictException("Doctor with this Phone number already exists");
            }
        }
    }

    private async Task SpecialitiesCheck(Guid specialityId)
    {
        var specialityEntity = await _context.Speciality.Where(s => s.Id == specialityId).FirstOrDefaultAsync();
        if (specialityEntity == null)
        {
            throw new KeyNotFoundException($"speciality with id {specialityId} not found");
        }
    }

    private static void CheckGender(Gender gender)
    {
        if (gender is Gender.Male or Gender.Female) return;

        throw new BadHttpRequestException(
            $"Possible Gender values: {Gender.Male.ToString()}, {Gender.Female.ToString()}");
    }

    private static void CheckBirthDate(DateTime? birthDate)
    {
        if (birthDate == null || birthDate <= DateTime.UtcNow) return;

        throw new BadHttpRequestException("Birth date can't be later than today");
    }
    
    private async Task<ClaimsIdentity> GetIdentity(string email, string password)
    {
        var userEntity = await _context.Doctor
            .FirstOrDefaultAsync(x => x.Email == email);

        if (userEntity == null)
        {
            throw new BadHttpRequestException("Wrong email address");
        }
        
        if (!CheckHashPassword(userEntity.Password, password))
        {
            throw new BadHttpRequestException("Wrong password");
        }
        
        var claims = new List<Claim>
        {
            new(ClaimsIdentity.DefaultNameClaimType, userEntity.Id.ToString())
        };

        var claimsIdentity = new ClaimsIdentity
        (
            claims,
            "Token",
            ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType
        );

        return claimsIdentity;
    }
    
    private static bool CheckHashPassword(string hashedPassword, string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
    
    private static void CheckPassword(string password)
    {
        if (password.Length < 6)
        {
            throw new BadHttpRequestException("Password must not be less than 6 characters ");
        }
        
        var regex = new Regex("^[a-zA-Z0-9!?]*$");

        if (!regex.IsMatch(password))
        {
            throw new BadHttpRequestException("Password can contain only letters and numbers and ! ?");
        }
    }
}
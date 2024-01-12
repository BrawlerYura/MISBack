using System;
using System.Collections.Generic;

namespace MISBack.Migrations;

public partial class Icd10
{
    public int? Actual { get; set; }

    public int? AddlCode { get; set; }

    public DateTime? Date { get; set; }

    public int? IdParent { get; set; }

    public string? MkbCode { get; set; }

    public string? MkbName { get; set; }

    public string? RecCode { get; set; }

    public int Id { get; set; }
}

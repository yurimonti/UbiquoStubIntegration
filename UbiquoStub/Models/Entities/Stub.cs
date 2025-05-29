using System;

namespace UbiquoStub.Models.Entities;

public class Stub
{
    public long Id {get;set;}
    public string Name {get;set;}
    public string? TestName { get; set; } = null;
    public int? Order { get; set; } = null;

    // public string TestName { get; set; }
    public string Host { get; set; }
    #region 1-to-1 with RequestEntity
    public RequestEntity Request {get;set;}
    #endregion
    
    #region 1-to-1 with Response
    public ResponseEntity Response {get;set;}
    #endregion
}

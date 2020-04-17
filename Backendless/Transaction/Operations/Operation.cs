﻿using System;
using Weborb.Service;

namespace BackendlessAPI.Transaction.Operations
{
  public abstract class Operation
  {
    private OperationType operationType;
    private String table;
    private String opResultId;

    public Operation()
    {
    }

    public Operation( OperationType operationType, String table, String opResultId )
    {
      this.operationType = operationType;
      this.table = table;
      this.opResultId = opResultId;
    }

    [SetClientClassMemberName("operationType")]
    public OperationType OperationType
    {
      get => operationType;
      set => operationType = value;
    }

    [SetClientClassMemberName("table")]
    public String Table
    {
      get => table;
      set => table = value;
    }

    [SetClientClassMemberName("opResultId")]
    public String OpResultId
    {
      get => opResultId;
      set => opResultId = value;
    }
    [SetClientClassMemberName("payload")]
    public abstract Object Payload{ get; set; } 

    public override string ToString()
    {
      return "Operation{operationType=" + operationType + ", table=" + table + ", opResultId=" + opResultId + ", payload=" + Payload + "}";
    }
  }
}

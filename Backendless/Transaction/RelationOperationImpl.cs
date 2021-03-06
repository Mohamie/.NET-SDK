﻿using BackendlessAPI.Exception;
using BackendlessAPI.Transaction.Operations;
using BackendlessAPI.Transaction.Payload;
using System;
using System.Collections.Generic;


namespace BackendlessAPI.Transaction
{
  class RelationOperationImpl
  {
    private LinkedList<Operation> operations;
    private OpResultIdGenerator opResultIdGenerator;

    public RelationOperationImpl( LinkedList<Operation> operations, OpResultIdGenerator opResultIdGenerator )
    {
      this.operations = operations;
      this.opResultIdGenerator = opResultIdGenerator;
    }

    public OpResult AddOperation( OperationType operationType, String parentTable,
                               Dictionary<String, Object> parentObject, String columnName, String[] childrenObjectIds )
    {
      String parentObjectId = TransactionHelper.ConvertObjectMapToObjectId( parentObject );

      if( childrenObjectIds == null || childrenObjectIds.Length == 0 )
        throw new ArgumentException( ExceptionMessage.NULL_BULK );

      return AddOperation( operationType, parentTable, parentObjectId, columnName, null, childrenObjectIds );
    }

    public OpResult AddOperation<E>( OperationType operationType, String parentTable, Dictionary<String, Object> parentObject,
                                                                                    String columnName, List<E> childrenInstances )
    {
      if( childrenInstances == null || childrenInstances.Count == 0 )
        throw new ArgumentException( ExceptionMessage.NULL_BULK );

      String parentObjectIds = TransactionHelper.ConvertObjectMapToObjectId( parentObject );

      return AddOperation( operationType, parentTable, parentObject, columnName, childrenInstances );
    }

    public OpResult AddOperation( OperationType operationType, String parentTable, Dictionary<String, Object> parentObject,
                                                          String columnName, List<Dictionary<String, Object>> childrenMaps )
    {
      String parentObjectId = TransactionHelper.ConvertObjectMapToObjectId( parentObject );
      return AddOperation( operationType, parentTable, parentObjectId, columnName, childrenMaps );
    }

    public OpResult AddOperation( OperationType operationType, String parentTable, Dictionary<String, Object> parentObject,
                                                                              String columnName, OpResult children )
    {
      String parentObjectId = TransactionHelper.ConvertObjectMapToObjectId( parentObject );
      return AddOperation( operationType, parentTable, parentObjectId, columnName, children );
    }

    public OpResult AddOperation( OperationType operationType, String parentTable, Dictionary<String, Object> parentObject,
                                                                  String columnName, String whereClauseForChildren )
    {
      String parentObjectId = TransactionHelper.ConvertObjectMapToObjectId( parentObject );
      return AddOperation( operationType, parentTable, parentObjectId, columnName, whereClauseForChildren );
    }

    public OpResult AddOperation( OperationType operationType, String parentTable, String parentObjectId, String columnName,
                                                                                         String[] childrenObjectIds )
    {
      if( childrenObjectIds == null || childrenObjectIds.Length == 0 )
        throw new ArgumentException( ExceptionMessage.NULL_BULK );

      return AddOperation( operationType, parentTable, parentObjectId, columnName, null, childrenObjectIds );
    }

    public OpResult AddOperation<E>( OperationType operationType, String parentTable, String parentObjectId,
                                                                   String columnName, List<E> childrenInstances )
    {
      if( childrenInstances == null || childrenInstances.Count == 0 )
        throw new ArgumentException( ExceptionMessage.NULL_BULK );

      List<String> childrenIds = GetChildrenFromArrayInstances( childrenInstances );

      return AddOperation( operationType, parentTable, parentObjectId, columnName, null, childrenIds );
    }

    public OpResult AddOperation( OperationType operationType, String parentTable, String parentObjectId, String columnName,
                                                                      List<Dictionary<String, Object>> childrenMaps )
    {
      List<Object> childrenIds = GetChildrenFromListMap( childrenMaps );

      return AddOperation( operationType, parentTable, parentObjectId, columnName, null, childrenIds );
    } 

    public OpResult AddOperation( OperationType operationType, String parentTable, String parentObjectId, String columnName,
                                                                                                  OpResult children )
    {
      CheckOpResultForChildren( children );

      return AddOperation( operationType, parentTable, parentObjectId, columnName, null, children.MakeReference() );
    }

    public OpResult AddOperation( OperationType operationType, String parentTable, String parentObjectId, String columnName, String whereClauseForChildren )
    {
      return AddOperation( operationType, parentTable, parentObjectId, columnName, whereClauseForChildren, null );
    }

    public OpResult AddOperation<E>( OperationType operationType, E parentObject, String columnName, 
                                                                        String[] childrenObjectIds )
    {
      String parentObjectId = GetParentObjectIdFromInstance( parentObject );
      String parentTable = parentObject.GetType().Name;

      if( childrenObjectIds == null || childrenObjectIds.Length == 0 )
        throw new ArgumentException( ExceptionMessage.NULL_BULK );

      return AddOperation( operationType, parentTable, parentObjectId, columnName, null, childrenObjectIds );
    }

    public OpResult AddOperation<E, U>( OperationType operationType, E parentObject, String columnName, List<U> childrenInstances )
    {
      if( childrenInstances == null || childrenInstances.Count == 0 )
        throw new ArgumentException( ExceptionMessage.NULL_BULK );

      String parentObjectId = GetParentObjectIdFromInstance( parentObject );
      String parentTable = parentObject.GetType().Name;
      List<String> childrenIds = GetChildrenFromArrayInstances( childrenInstances );

      return AddOperation( operationType, parentTable, parentObjectId, columnName, null, childrenIds );
    }

    public OpResult AddOperation<E>( OperationType operationType, E parentObject, String columnName,
                                                      List<Dictionary<String, Object>> childrenMaps )
    {
      String parentObjectId = GetParentObjectIdFromInstance( parentObject );
      String parentTable = parentObject.GetType().Name;
      List<Object> childrenIds = GetChildrenFromListMap( childrenMaps );

      return AddOperation( operationType, parentTable, parentObjectId, columnName, null, childrenIds );
    }

    public OpResult AddOperation<E>( OperationType operationType, E parentObject, String columnName, OpResult children )
    {
      String parentObjectId = GetParentObjectIdFromInstance( parentObject );
      String parentTable = parentObject.GetType().Name;
      CheckOpResultForChildren( children );

      return AddOperation( operationType, parentTable, parentObjectId, columnName,
                           null, children.MakeReference() );
    }

    public OpResult AddOperation<E>( OperationType operationType, E parentObject, String columnName,
                                                                     String whereClauseForChildren )
    {
      String parentObjectId = GetParentObjectIdFromInstance( parentObject );
      String parentTable = parentObject.GetType().Name;

      return AddOperation( operationType, parentTable, parentObjectId, columnName, whereClauseForChildren, null );
    }

    public OpResult AddOperation( OperationType operationType, OpResult parentObject, String columnName, String[] childrenObjectIds )
    {
      if( childrenObjectIds == null || childrenObjectIds.Length == 0 )
        throw new ArgumentException( ExceptionMessage.NULL_BULK );

      return AddOperation( operationType, parentObject.TableName,
                           parentObject.ResolveTo( "objectId" ).MakeReference(),
                           columnName, null, childrenObjectIds );
    }

    public OpResult AddOperation<E>( OperationType operationType, OpResult parentObject, String columnName,
                                                                                    List<E> childrenInstances )
    {
      if( childrenInstances == null || childrenInstances.Count == 0 )
        throw new ArgumentException( ExceptionMessage.NULL_BULK );

      CheckOpResultForParent( parentObject );
      List<String> childrenIds = GetChildrenFromArrayInstances( childrenInstances );

      return AddOperation( operationType, parentObject.TableName,
                           parentObject.ResolveTo( "objectId" ).MakeReference(),
                           columnName, null, childrenIds );
    }

    public OpResult AddOperation( OperationType operationType, OpResult parentObject, String columnName,
                                                          List<Dictionary<String, Object>> childrenMaps )
    {
      CheckOpResultForParent( parentObject );
      List<Object> childrenIds = GetChildrenFromListMap( childrenMaps );

      return AddOperation( operationType, parentObject.TableName,
                           parentObject.ResolveTo( "objectId" ).MakeReference(),
                           columnName, null, childrenIds );
    }

    public OpResult AddOperation( OperationType operationType, OpResult parentObject, String columnName, OpResult children )
    {
      CheckOpResultForParent( parentObject );
      CheckOpResultForChildren( children );

      return AddOperation( operationType, parentObject.TableName,
                           parentObject.ResolveTo( "objectId" ).MakeReference(),
                           columnName, null, children.MakeReference() );
    }

    public OpResult AddOperation( OperationType operationType, OpResult parentObject, String columnName,
                                                                         String whereClauseForChildren )
    {
      CheckOpResultForParent( parentObject );

      return AddOperation( operationType, parentObject.TableName,
                           parentObject.ResolveTo( "objectId" ).MakeReference(),
                           columnName, whereClauseForChildren, null );
    }

    public OpResult AddOperation( OperationType operationType, OpResultValueReference parentObject, String columnName, String[] childrenObjectIds )
    {
      Dictionary<String, Object> referenceToObjectId = GetReferenceToParentFromOpResultValue( parentObject );

      if( childrenObjectIds == null || childrenObjectIds.Length == 0 )
        throw new ArgumentException( ExceptionMessage.NULL_BULK );

      return AddOperation( operationType, parentObject.OpResult.TableName, referenceToObjectId, columnName,
                           null, childrenObjectIds );
    }

    public OpResult AddOperation<E>( OperationType operationType, OpResultValueReference parentObject, String columnName,
                                                                                           List<E> childrenInstances )
    {
      if( childrenInstances == null || childrenInstances.Count == 0 )
        throw new ArgumentException( ExceptionMessage.NULL_BULK );

      Dictionary<String, Object> referenceToObjectId = GetReferenceToParentFromOpResultValue( parentObject );
      List<String> childrenIds = GetChildrenFromArrayInstances( childrenInstances );

      return AddOperation( operationType, parentObject.OpResult.TableName, referenceToObjectId, columnName,
                           null, childrenIds );
    }

    public OpResult AddOperation( OperationType operationType, OpResultValueReference parentObject, String columnName,
                                                                       List<Dictionary<String, Object>> childrenMaps )
    {
      Dictionary<String, Object> referenceToObjectId = GetReferenceToParentFromOpResultValue( parentObject );
      List<Object> childrenIds = GetChildrenFromListMap( childrenMaps );

      return AddOperation( operationType, parentObject.OpResult.TableName, referenceToObjectId, columnName,
                           null, childrenIds );
    }

    public OpResult AddOperation( OperationType operationType, OpResultValueReference parentObject, String columnName, OpResult children )
    {
      Dictionary<String, Object> referenceToObjectId = GetReferenceToParentFromOpResultValue( parentObject );
      CheckOpResultForChildren( children );

      return AddOperation( operationType, parentObject.OpResult.TableName, referenceToObjectId, columnName,
                           null, children.MakeReference() );
    }

    public OpResult AddOperation( OperationType operationType, OpResultValueReference parentObject, String columnName, String whereClauseForChildren )
    {
      Dictionary<String, Object> referenceToObjectId = GetReferenceToParentFromOpResultValue( parentObject );

      return AddOperation( operationType, parentObject.OpResult.TableName, referenceToObjectId, columnName,
                           whereClauseForChildren, null );
    }

    private OpResult AddOperation( OperationType operationType, String parentTable, Object parentObject, String columnName,
                                                                            String whereClauseForChildren, Object children )
    {
      if( parentTable == null || parentTable == "" )
        throw new ArgumentException( ExceptionMessage.NULL_PARENT_TABLE_NAME );

      if( columnName == null || columnName == "" )
        throw new ArgumentException( ExceptionMessage.NULL_RELATION_COLUMN_NAME );

      String operationResultId = opResultIdGenerator.GenerateOpResultId( operationType, parentTable );

      Relation relation = new Relation();
      relation.ParentObject = parentObject;
      relation.RelationColumn = columnName;
      relation.Conditional = whereClauseForChildren;
      relation.Unconditional = children;

      switch( operationType )
      {
        case OperationType.ADD_RELATION:
          operations.AddLast( new OperationAddRelation( operationType, parentTable, operationResultId, relation ) );
          break;

        case OperationType.SET_RELATION:
          operations.AddLast( new OperationSetRelation( operationType, parentTable, operationResultId, relation ) );
          break;

        case OperationType.DELETE_RELATION:
          operations.AddLast( new OperationDeleteRelation( operationType, parentTable, operationResultId, relation ) );
          break;
      }

      return TransactionHelper.MakeOpResult( parentTable, operationResultId, operationType );
    }

    private void CheckOpResultForParent( OpResult parentObject )
    {
      if( parentObject == null )
        throw new ArgumentException( ExceptionMessage.NULL_ENTITY );

      if( !OperationTypeUtil.supportCollectionEntityDescriptionType.Contains( parentObject.OperationType ) )
        throw new ArgumentException( ExceptionMessage.REF_TYPE_NOT_SUPPORT );
    }

    private String GetParentObjectIdFromInstance<E>( E parentObject )
    {
      if( parentObject == null )
        throw new ArgumentException( ExceptionMessage.NULL_ENTITY );

      String parentObjectId = TransactionHelper.GetObjectIdFromInstance( parentObject );

      if( parentObjectId == null )
        throw new ArgumentException( ExceptionMessage.NULL_OBJECT_ID_IN_INSTANCE );

      return parentObjectId;
    }

    private Dictionary<String,Object> GetReferenceToParentFromOpResultValue( OpResultValueReference parentObject )
    {
      if( parentObject == null )
        throw new ArgumentException( ExceptionMessage.NULL_OP_RESULT_VALUE_REFERENCE );

      if( parentObject.ResultIndex == null || parentObject.PropName == null )
        throw new ArgumentException( ExceptionMessage.OP_RESULT_INDEX_YES_PROP_NAME_NOT );

      return TransactionHelper.ConvertCreateBulkOrFindResultIndexToObjectId( parentObject );
    }

    private void CheckOpResultForChildren( OpResult children )
    {
      if( children == null )
        throw new ArgumentException( ExceptionMessage.NULL_OP_RESULT );

      if( !( OperationTypeUtil.supportCollectionEntityDescriptionType.Contains( children.OperationType ) ||
            OperationTypeUtil.supportListIdsResultType.Contains( children.OperationType ) ) )
        throw new ArgumentException( ExceptionMessage.REF_TYPE_NOT_SUPPORT );
    }

    private List<Object> GetChildrenFromListMap( List<Dictionary<String, Object>> childrenMaps )
    {
      if( childrenMaps == null )
        throw new ArgumentException( ExceptionMessage.NULL_BULK );

      return TransactionHelper.ConvertMapsToObjectIds( childrenMaps );
    }

    private List<String> GetChildrenFromArrayInstances<E>( List<E> children )
    {
      if( children == null || children.Count == 0 )
        throw new ArgumentException( ExceptionMessage.NULL_BULK );

      return TransactionHelper.GetObjectIdsFromListInstaces( children.ToArray() );
    }
  }
}

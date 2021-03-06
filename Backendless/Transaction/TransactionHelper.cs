﻿using BackendlessAPI.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BackendlessAPI.Transaction
{
  class TransactionHelper
  {
    private static String LAST_LOGIN_COLUMN_NAME = "lastLogin";
    private static String PASSWORD_KEY = "password";
    private static String SOCIAL_ACCOUNT_COLUMN_NAME = "socialAccount";
    private static String USER_STATUS_COLUMN_NAME = "userStatus";

    internal static void RemoveSystemField( Dictionary<String, Object> changes )
    {
      changes.Remove( LAST_LOGIN_COLUMN_NAME );
      changes.Remove( PASSWORD_KEY );
      changes.Remove( SOCIAL_ACCOUNT_COLUMN_NAME );
      changes.Remove( USER_STATUS_COLUMN_NAME );
      changes.Remove( "objectId" );
      changes.Remove( "created" );
      changes.Remove( "updated" );
    }

    static internal OpResult MakeOpResult( String tableName, String operationResultId, OperationType operationType )
    {
      return new OpResult( tableName, operationResultId, operationType );
    }

    internal static List<Object> ConvertMapsToObjectIds( List<Dictionary<String, Object>> objectMaps )
    {
      List<Object> objectIds = new List<Object>();

      foreach( Dictionary<String, Object> objectMap in objectMaps )
        objectIds.Add( ConvertObjectMapToObjectIdOrLeaveReference( objectMap ) );

      return objectIds;
    }

    internal static Dictionary<String, Object> ConvertInstanceToMap<E>( E instance )
    {
      if( instance == null )
        throw new ArgumentException( ExceptionMessage.NULL_INSTANCE );

      Dictionary<String, Object> entity = new Dictionary<String, Object>();
      Type fieldsType = typeof( E );
      FieldInfo[] fields = fieldsType.GetFields( BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance );

      foreach( FieldInfo field in fields )
        entity[ field.Name ] = field.GetValue( instance );

      return entity;
    }


    internal static String ConvertObjectMapToObjectId( Dictionary<String, Object> objectMap )
    {
      if( objectMap == null )
        throw new ArgumentException( ExceptionMessage.NULL_MAP );

      Object maybeObjectId = objectMap[ "objectId" ];

      if( !( maybeObjectId is String ) )
        throw new ArgumentException( ExceptionMessage.NULL_OBJECT_ID_IN_OBJECT_MAP );

      return (String) maybeObjectId;
    }
    
    internal static String GetObjectIdFromInstance<E>( E instance )
    {
      if( instance == null )
        throw new ArgumentException( ExceptionMessage.NULL_INSTANCE );

      Type fieldsType = typeof( E );
      FieldInfo[] fields = fieldsType.GetFields( BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance );

      foreach( FieldInfo field in fields ) 
        if( field.Name == "objectId" || field.Name == "ObjectId" )
          return (String) field.GetValue( instance );

      throw new ArgumentException( ExceptionMessage.NULL_OBJECT_ID_IN_INSTANCE );
    }
    internal static List<String> GetObjectIdsFromListInstaces<E>( E[] instances )
    {
      if( instances[ 0 ] is Dictionary<String, Object> )
        throw new ArgumentException( ExceptionMessage.RELATION_USE_LIST_OF_MAPS );

      else if( !( instances[ 0 ].GetType().IsArray ) )
      {
        List<String> objectIds = new List<String>();

        foreach( E entry in instances )
          objectIds.Add( GetObjectIdFromInstance( entry ) );

        return objectIds;
      }
      else
        throw new ArgumentException( ExceptionMessage.LIST_NOT_INSTANCES );
    }

    static Object ConvertObjectMapToObjectIdOrLeaveReference( Dictionary<String, Object> objectMap )
    {
      if( objectMap == null )
        throw new ArgumentException( ExceptionMessage.NULL_MAP );

      if( objectMap.ContainsKey( UnitOfWork.REFERENCE_MARKER ) )
      {
        objectMap[ UnitOfWork.PROP_NAME ] = "objectId";
        return objectMap;
      }

      Object maybeObjectId = objectMap[ "objectId" ];

      if( !( maybeObjectId is String ) )
        throw new ArgumentException( ExceptionMessage.NULL_OBJECT_ID_IN_OBJECT_MAP );

      return maybeObjectId;
    }

    internal static Dictionary<String, Object> ConvertCreateBulkOrFindResultIndexToObjectId( OpResultValueReference parentObject )
    {
      Dictionary<String, Object> referenceToObjectId;

      if( OperationTypeUtil.supportCollectionEntityDescriptionType.Contains( parentObject.OpResult.OperationType ) )
        referenceToObjectId = parentObject.ResolveTo( "objectId" ).MakeReference();
      else if( OperationTypeUtil.supportListIdsResultType.Contains( parentObject.OpResult.OperationType ) )
        referenceToObjectId = parentObject.MakeReference();
      else
        throw new ArgumentException( ExceptionMessage.REF_TYPE_NOT_SUPPORT );

      return referenceToObjectId;
    }

    internal static void MakeReferenceToValueFromOpResult( Dictionary<String, Object> map )
    {
      foreach( KeyValuePair<String, Object> kvp in map )
      {
        Dictionary<String, Object> entry = new Dictionary<String, Object>( map );

        if( entry[ kvp.Key ] is OpResult )
          if( OperationTypeUtil.supportIntResultType.Contains( ( (OpResult) kvp.Value ).OperationType ) )
            entry[ kvp.Key ] = ( (OpResult) kvp.Value ).MakeReference();
          else
            throw new ArgumentException( ExceptionMessage.OP_RESULT_FROM_THIS_OPERATION_NOT_SUPPORT_IN_THIS_PLACE );

        if( entry[ kvp.Key ] is OpResultValueReference )
        {
          OpResultValueReference reference = (OpResultValueReference) kvp.Value;

          if( IsCreatedUpdatedPropName( reference ) || IsCreatedBulkResultIndex( reference ) || IsFoundPropNameResultIndex( reference ) )
            entry[ kvp.Key ] = reference.MakeReference();
          else
            throw new ArgumentException( ExceptionMessage.OP_RESULT_FROM_THIS_OPERATION_NOT_SUPPORT_IN_THIS_PLACE );      
        }
      }
    }

    internal static void MakeReferenceToObjectIdFromOpResult( List<Object> listObjectIds )
    {
      IEnumerator<Object> iterator = listObjectIds.GetEnumerator();

      while( iterator.MoveNext() )
      {
        Object tempEntity = iterator.MoveNext();

        if( tempEntity is OpResult )
          throw new ArgumentException( ExceptionMessage.OP_RESULT_FROM_THIS_OPERATION_NOT_SUPPORT_IN_THIS_PLACE );

        if( tempEntity is OpResultValueReference )
        {
          OpResultValueReference reference = (OpResultValueReference) tempEntity;

          if( IsCreatedUpdatedObjectId( reference ) || IsCreatedBulkResultIndex( reference ) || IsFoundPropNameResultIndex( reference ) )
            tempEntity = reference.MakeReference();
          else
            throw new ArgumentException( ExceptionMessage.OP_RESULT_FROM_THIS_OPERATION_NOT_SUPPORT_IN_THIS_PLACE );
        }
      }
    }

    private static Boolean IsCreatedUpdatedObjectId( OpResultValueReference reference )
    {
      return IsCreatedUpdatedPropName( reference ) && reference.PropName.Equals( "objectId" );
    }

    private static Boolean IsCreatedUpdatedPropName( OpResultValueReference reference )
    {
      return OperationTypeUtil.supportEntityDescriptionResultType.Contains( reference.OpResult.OperationType ) &&
                                                                      reference.PropName == null &&
                                                                      reference.ResultIndex != null;
    }

    private static Boolean IsCreatedBulkResultIndex( OpResultValueReference reference )
    {
      return OperationType.CREATE_BULK.Equals( reference.OpResult.OperationType ) &&
            reference.PropName == null &&
            reference.ResultIndex != null;
    }

    private static Boolean IsFoundResultIndexObjectId( OpResultValueReference reference )
    {
      return IsFoundPropNameResultIndex( reference ) && reference.PropName.Equals( "objectId" );
    }

    private static Boolean IsFoundPropNameResultIndex( OpResultValueReference reference )
    {
      return OperationType.FIND.Equals( reference.OpResult.OperationType ) &&
            reference.PropName != null &&
            reference.ResultIndex != null;
    }
  }
}

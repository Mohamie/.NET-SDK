﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using BackendlessAPI;
using BackendlessAPI.Persistence;
using System;
using System.Collections.Generic;
using BackendlessAPI.Transaction;

namespace TestProject
{
  [TestClass]
  public class TestTransactionAddRelation
  {
    [TestCleanup]
    public void TearDown()
    {
      Backendless.Data.Of( "Person" ).Remove( "age = '22'" );
      Backendless.Data.Of( "Order" ).Remove( "LastName = 'Smith'" );
    }

    [TestMethod]
    public void TestAddRelation_Class()
    {
      List<Person> listPerson = new List<Person>();
      Person personObj = new Person();
      personObj.age = 22;
      personObj.name = "Eva";
      listPerson.Add( personObj );

      IList<String> parentObjIds = Backendless.Data.Of<Person>().Create( listPerson );
      listPerson.Clear();
      personObj.objectId = parentObjIds[ 0 ];

      List<Order> listOrder = new List<Order>();
      Order orderObj = new Order();
      orderObj.LastName = "Smith";
      listOrder.Add( orderObj );

      IList<String> childObjIds = Backendless.Data.Of<Order>().Create( listOrder );
      listOrder.Clear();
      orderObj.objectId = childObjIds[ 0 ];
      listOrder.Add( orderObj );

      String relationColumn = "Surname";

      UnitOfWork uow = new UnitOfWork();
      uow.AddToRelation( personObj, relationColumn, listOrder );
      UnitOfWorkResult uowResult = uow.Execute();

      Assert.IsTrue( uowResult.Success );
      Assert.IsNotNull( uowResult.Results );

      DataQueryBuilder dqb = DataQueryBuilder.Create().SetRelationsDepth( 10 );
      dqb.SetRelationsPageSize( 10 );

      IList<Person> listCheckPersonObj = Backendless.Data.Of<Person>().Find( dqb );
      Assert.IsTrue( listCheckPersonObj.Count == 1 );
      Assert.IsTrue( listCheckPersonObj[ 0 ].Surname != null );
    }

    [TestMethod]
    public void TestAddRelation_Dictionary()
    {
      List<Person> listPerson = new List<Person>();
      Person personObj = new Person();
      personObj.age = 22;
      personObj.name = "Eva";
      listPerson.Add( personObj );

      IList<String> parentObjIds = Backendless.Data.Of<Person>().Create( listPerson );
      listPerson.Clear();
      personObj.objectId = parentObjIds[ 0 ];

      List<Order> listOrder = new List<Order>();
      Order orderObj = new Order();
      orderObj.LastName = "Smith";
      listOrder.Add( orderObj );
      List<Dictionary<String, Object>> listChildObjMap = new List<Dictionary<String, Object>>();
      Dictionary<String, Object> childObjMap = new Dictionary<String, Object>();
      childObjMap[ "LastName" ] = "Smith";
      listChildObjMap.Add( childObjMap );

      IList<String> childObjIds = Backendless.Data.Of( "Order" ).Create( listChildObjMap );
      listChildObjMap.Clear();
      childObjMap[ "objectId" ] = childObjIds[ 0 ];
      listChildObjMap.Add( childObjMap );

      String relationColumn = "Surname";

      UnitOfWork uow = new UnitOfWork();
      uow.AddToRelation( personObj.GetType().Name, personObj.objectId, relationColumn, listChildObjMap );
      UnitOfWorkResult uowResult = uow.Execute();

      Assert.IsTrue( uowResult.Success );
      Assert.IsNotNull( uowResult.Results );

      DataQueryBuilder dqb = DataQueryBuilder.Create();
      dqb.SetRelationsPageSize( 10 );
      dqb.SetRelationsDepth( 10 );
      IList<Person> listCheckPersonObj = Backendless.Data.Of<Person>().Find( dqb );

      Assert.IsTrue( listCheckPersonObj.Count == 1 );
      Assert.IsTrue( listCheckPersonObj[0].Surname != null );
    }

    [TestMethod]
    public void TestAddRelation_OpResult()
    {
      List<Person> listPerson = new List<Person>();
      Person personObj = new Person();
      personObj.age = 22;
      personObj.name = "Eva";
      listPerson.Add( personObj );

      IList<String> parentObjIds = Backendless.Data.Of<Person>().Create( listPerson );
      listPerson.Clear();
      personObj.objectId = parentObjIds[ 0 ];

      List<Order> listOrder = new List<Order>();
      Order orderObj = new Order();
      orderObj.LastName = "Smith";
      listOrder.Add( orderObj );
      List<Dictionary<String, Object>> listChildObjMap = new List<Dictionary<String, Object>>();
      Dictionary<String, Object> childObjMap = new Dictionary<String, Object>();
      childObjMap[ "LastName" ] = "Smith";
      listChildObjMap.Add( childObjMap );

      IList<String> childObjIds = Backendless.Data.Of( "Order" ).Create( listChildObjMap );
      listChildObjMap.Clear();
      childObjMap[ "objectId" ] = childObjIds[ 0 ];
      listChildObjMap.Add( childObjMap );

      UnitOfWork uow = new UnitOfWork();

      DataQueryBuilder queryBuilder = DataQueryBuilder.Create();
      queryBuilder.SetWhereClause( "LastName in ('Smith')" );

      OpResult gifts = uow.Find( "Order", queryBuilder );
      String relationColumn = "Surname";

      uow.AddToRelation( personObj.GetType().Name, personObj.objectId, relationColumn, gifts );
      UnitOfWorkResult uowResult = uow.Execute();

      Assert.IsTrue( uowResult.Success );
      Assert.IsNotNull( uowResult.Results );

      DataQueryBuilder dqb = DataQueryBuilder.Create();
      dqb.SetRelationsPageSize( 10 );
      dqb.SetRelationsDepth( 10 );
      IList<Person> listCheckPersonObj = Backendless.Data.Of<Person>().Find( dqb );

      Assert.IsTrue( listCheckPersonObj.Count == 1 );
      Assert.IsTrue( listCheckPersonObj[ 0 ].Surname != null );
    }

    [TestMethod]
    public void TestAddRelation_WithId()
    {
      List<Person> listPerson = new List<Person>();
      Person personObj = new Person();
      personObj.age = 22;
      personObj.name = "Eva";
      listPerson.Add( personObj );

      IList<String> parentObjIds = Backendless.Data.Of<Person>().Create( listPerson );
      listPerson.Clear();
      personObj.objectId = parentObjIds[ 0 ];

      List<Order> listOrder = new List<Order>();
      Order orderObj = new Order();
      orderObj.LastName = "Smith";
      listOrder.Add( orderObj );
      List<Dictionary<String, Object>> listChildObjMap = new List<Dictionary<String, Object>>();
      Dictionary<String, Object> childObjMap = new Dictionary<String, Object>();
      childObjMap[ "LastName" ] = "Smith";
      listChildObjMap.Add( childObjMap );

      IList<String> childObjIds = Backendless.Data.Of( "Order" ).Create( listChildObjMap );
      listChildObjMap.Clear();
      childObjMap[ "objectId" ] = childObjIds[ 0 ];
      listChildObjMap.Add( childObjMap );

      String relationColumn = "Surname";

      UnitOfWork uow = new UnitOfWork();
      uow.AddToRelation( personObj.GetType().Name, personObj.objectId, relationColumn, new String[] { (String) childObjMap[ "objectId" ] } );
      UnitOfWorkResult uowResult = uow.Execute();

      Assert.IsTrue( uowResult.Success );
      Assert.IsNotNull( uowResult.Results );

      DataQueryBuilder dqb = DataQueryBuilder.Create();
      dqb.SetRelationsPageSize( 10 );
      dqb.SetRelationsDepth( 10 );
      IList<Person> listCheckPersonObj = Backendless.Data.Of<Person>().Find( dqb );

      Assert.IsTrue( listCheckPersonObj.Count == 1 );
      Assert.IsTrue( listCheckPersonObj[ 0 ].Surname != null );
    }

    [TestMethod]
    public void TestAddToRelation_CheckError()
    {
      List<Person> listPerson = new List<Person>();
      Person personObj = new Person();
      personObj.age = 22;
      personObj.name = "Eva";
      listPerson.Add( personObj );

      IList<String> parentObjIds = Backendless.Data.Of<Person>().Create( listPerson );
      listPerson.Clear();
      personObj.objectId = parentObjIds[ 0 ];

      List<Order> listOrder = new List<Order>();
      Order orderObj = new Order();
      orderObj.LastName = "Smith";
      listOrder.Add( orderObj );
      List<Dictionary<String, Object>> listChildObjMap = new List<Dictionary<String, Object>>();
      Dictionary<String, Object> childObjMap = new Dictionary<String, Object>();
      childObjMap[ "LastName" ] = "Smith";
      listChildObjMap.Add( childObjMap );

      IList<String> childObjIds = Backendless.Data.Of( "Order" ).Create( listChildObjMap );
      listChildObjMap.Clear();
      childObjMap[ "objectId" ] = childObjIds[ 0 ];
      listChildObjMap.Add( childObjMap );

      String relationColumn = "Surname";

      UnitOfWork uow = new UnitOfWork();
      uow.AddToRelation( "Wrong name", personObj.objectId, relationColumn, new String[] { (String) childObjMap[ "objectId" ] } );
      UnitOfWorkResult uowResult = uow.Execute();

      Assert.IsFalse( uowResult.Success );
      Assert.IsNull( uowResult.Results );
    }
  }
}

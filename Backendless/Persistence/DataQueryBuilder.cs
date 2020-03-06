﻿using System;
using System.Collections.Generic;

namespace BackendlessAPI.Persistence
{
  public class DataQueryBuilder
  {
    private PagedQueryBuilder<DataQueryBuilder> pagedQueryBuilder;
    private QueryOptionsBuilder<DataQueryBuilder> queryOptionsBuilder;
    private List<String> properties;
    private String whereClause;
    private List<string> groupBy;
    private String havingClause;

    private DataQueryBuilder()
    {
      properties = new List<String>();
      pagedQueryBuilder = new PagedQueryBuilder<DataQueryBuilder>( this );
      queryOptionsBuilder = new QueryOptionsBuilder<DataQueryBuilder>( this );
    }

    public static DataQueryBuilder Create()
    {
      return new DataQueryBuilder();
    }

    public BackendlessDataQuery Build()
    {
      BackendlessDataQuery dataQuery = pagedQueryBuilder.Build();
      dataQuery.QueryOptions = queryOptionsBuilder.Build();
      dataQuery.Properties = properties;
      dataQuery.WhereClause = whereClause;
 
      return dataQuery;
    }

    public DataQueryBuilder SetPageSize( int pageSize )
    {
      return pagedQueryBuilder.SetPageSize( pageSize );
    }

    public DataQueryBuilder SetOffset( int offset )
    {
      return pagedQueryBuilder.SetOffset( offset );
    }

    public DataQueryBuilder PrepareNextPage()
    {
      return pagedQueryBuilder.PrepareNextPage();
    }

    public DataQueryBuilder PreparePreviousPage()
    {
      return pagedQueryBuilder.PreparePreviousPage();
    }

    public List<String> GetProperties()
    {
      return properties;
    }

    public DataQueryBuilder SetProperties( List<String> properties )
    {
      this.properties = properties;
      return this;
    }

    public DataQueryBuilder AddProperty( String property )
    {
      this.properties.Add( property );
      return this;
    }

    public String GetWhereClause()
    {
      return whereClause;
    }

    public DataQueryBuilder SetWhereClause( String whereClause )
    {
      this.whereClause = whereClause;
      return this;
    }

    public List<String> GetSortBy()
    {
      return queryOptionsBuilder.GetSortBy();
    }

    public DataQueryBuilder SetSortBy( List<String> sortBy )
    {
      return queryOptionsBuilder.SetSortBy( sortBy );
    }

    public DataQueryBuilder AddSortBy( String sortBy )
    {
      return queryOptionsBuilder.AddSortBy( sortBy );
    }

    public List<String> GetRelated()
    {
      return queryOptionsBuilder.GetRelated();
    }

    public DataQueryBuilder AddRelated( string related )
    {
      return queryOptionsBuilder.AddRelated( related );
    }

    public DataQueryBuilder SetRelated( List<String> related )
    {
      return queryOptionsBuilder.SetRelated( related );
    }

    public int GetRelationsDepth()
    {
      return queryOptionsBuilder.GetRelationsDepth();
    }

    public DataQueryBuilder SetRelationsDepth( int relationsDepth )
    {
      return queryOptionsBuilder.SetRelationsDepth( relationsDepth );
    }

    public int GetRelationsPageSize()
    {
        return queryOptionsBuilder.GetRelationsPageSize();
    }
    public DataQueryBuilder SetRelationsPageSize( int relationsPageSize )
    {
        return queryOptionsBuilder.SetRelationsPageSize( relationsPageSize );
    }

    public DataQueryBuilder SetGroupBy( List<String> groupBy )
    {
      this.groupBy = new List<String>();
      this.groupBy.AddRange( groupBy );
      return this;
    }

    public DataQueryBuilder AddGroupdBy( List<String> groupdBy )
    {
      this.groupBy = this.groupBy != null ? this.groupBy : new List<String>();
      this.groupBy.AddRange( groupBy );
      return this;
    }

    public DataQueryBuilder SetHavingClause( String havingClause )
    {
      this.havingClause = havingClause;
      return this;
    }
  }
}

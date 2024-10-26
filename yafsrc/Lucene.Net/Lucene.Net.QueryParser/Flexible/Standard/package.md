---
uid: Lucene.Net.QueryParsers.Flexible.Standard
summary: *content
---

<!--
 Licensed to the Apache Software Foundation (ASF) under one or more
 contributor license agreements.  See the NOTICE file distributed with
 this work for additional information regarding copyright ownership.
 The ASF licenses this file to You under the Apache License, Version 2.0
 (the "License"); you may not use this file except in compliance with
 the License.  You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.
-->


Implementation of the [Lucene classic query parser](xref:Lucene.Net.QueryParsers.Classic) using the flexible query parser frameworks

## Lucene Flexible Query Parser Implementation

The old Lucene query parser used to have only one class that performed all the parsing operations. In the new query parser structure, the parsing was divided in 3 steps: parsing (syntax), processing (semantic) and building. 

The classes contained in the namespace <xref:Lucene.Net.QueryParsers.Flexible.Standard> are used to reproduce the same behavior as the old query parser. 

Check <xref:Lucene.Net.QueryParsers.Flexible.Standard.StandardQueryParser> to quick start using the Lucene query parser. 
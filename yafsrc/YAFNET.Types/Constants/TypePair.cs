/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Types.Constants;

public readonly struct TypePair : IEquatable<TypePair>
{
    public Type Source { get; }
    public Type Target { get; }

    public TypePair(Type source, Type target)
    {
        this.Source = source;
        this.Target = target;
    }

    public bool Equals(TypePair other)
    {
        return this.Source == other.Source && this.Target == other.Target;
    }

    public override bool Equals(object obj)
    {
        return obj is TypePair other && this.Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Source, this.Target);
    }

    public static bool operator ==(TypePair left, TypePair right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(TypePair left, TypePair right)
    {
        return !(left == right);
    }
}
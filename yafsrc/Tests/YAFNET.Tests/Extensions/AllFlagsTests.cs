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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Tests.Extensions;

using System.Reflection;

using YAF.Types.Flags;

/// <summary>
/// Checks every flags class: each bit of its <c>Flags</c> enum maps to the bool property with the
/// same name (in both directions), and the constructors / implicit conversions agree.
/// </summary>
[TestFixture]
public class AllFlagsTests
{
    private static IEnumerable<Type> FlagTypes() =>
        typeof(FlagsBase).Assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(FlagsBase)) && !t.IsAbstract && t.GetNestedType("Flags") != null)
            .OrderBy(t => t.Name);

    [Test]
    public void All_Flag_Types_Are_Found_Test()
    {
        FlagTypes().Select(t => t.Name).Should().Contain(
            ["AccessFlags", "GroupFlags", "UserBlockFlags", "MedalFlags", "TopicFlags", "UserFlags", "ForumFlags"]);
    }

    [TestCaseSource(nameof(FlagTypes))]
    [Description("Each enum bit sets exactly the property with the same name and vice versa")]
    public void Properties_Match_Enum_Bits_Test(Type flagType)
    {
        var enumType = flagType.GetNestedType("Flags");
        var properties = flagType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType == typeof(bool) && p.GetIndexParameters().Length == 0 && p.CanWrite)
            .ToDictionary(p => p.Name);

        var matched = 0;

        foreach (var value in Enum.GetValues(enumType).Cast<object>())
        {
            var bit = Convert.ToInt32(value);
            var name = Enum.GetName(enumType, value);

            if (bit == 0 || !properties.TryGetValue(name!, out var property))
            {
                continue;
            }

            matched++;

            // enum -> property
            var fromEnum = (FlagsBase)Activator.CreateInstance(flagType, value);
            fromEnum.BitValue.Should().Be(bit);

            foreach (var other in properties.Values)
            {
                ((bool)other.GetValue(fromEnum)!).Should().Be(
                    other.Name == name,
                    $"{flagType.Name}.{other.Name} for bit {name}");
            }

            // property -> bit value, and clearing it again
            var fromProperty = Create(flagType, 0);
            property.SetValue(fromProperty, true);
            fromProperty.BitValue.Should().Be(bit, $"{flagType.Name}.{name}");
            property.SetValue(fromProperty, false);
            fromProperty.BitValue.Should().Be(0);
        }

        matched.Should().BeGreaterThan(0, $"{flagType.Name} should have properties for its flags");
    }

    private static FlagsBase Create(Type flagType, int bitValue)
    {
        return (FlagsBase)flagType.GetConstructor([typeof(int)])!.Invoke([bitValue]);
    }

    [TestCaseSource(nameof(FlagTypes))]
    [Description("int, object and bool[] constructors and the implicit conversions give the same bit value")]
    public void Constructors_And_Conversions_Test(Type flagType)
    {
        const int Bits = 5;

        Create(flagType, Bits).BitValue.Should().Be(Bits);

        // not every flags class has every constructor (MessageFlags e.g. defaults to html, bbcode and smilies)
        if (flagType.GetConstructor([typeof(object)]) is { } fromObject)
        {
            ((FlagsBase)fromObject.Invoke([(object)Bits])).BitValue.Should().Be(Bits);
        }

        if (flagType.GetConstructor([typeof(bool[])]) is { } fromBits)
        {
            ((FlagsBase)fromBits.Invoke([new[] { true, false, true }])).BitValue.Should().Be(Bits);
        }

        var conversions = flagType.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name == "op_Implicit" && m.ReturnType == flagType)
            .ToList();

        foreach (var conversion in conversions)
        {
            var parameterType = conversion.GetParameters()[0].ParameterType;
            var argument = parameterType == typeof(int) ? Bits : Enum.ToObject(parameterType, Bits);

            ((FlagsBase)conversion.Invoke(null, [argument])).BitValue.Should().Be(Bits, $"{flagType.Name} from {parameterType.Name}");
        }
    }
}

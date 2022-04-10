// ***********************************************************************
// <copyright file="FieldReference.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack.OrmLite
{
    using System;

    public class FieldReference
    {
        public FieldDefinition FieldDef { get; }

        public FieldReference(FieldDefinition fieldDef) => this.FieldDef = fieldDef;

        /// <summary>
        /// Foreign Key Table name
        /// </summary>
        public Type RefModel { get; set; }

        private ModelDefinition refModelDef;
        public ModelDefinition RefModelDef => this.refModelDef ??= this.RefModel.GetModelDefinition();

        /// <summary>
        /// The Field name on current Model to use for the Foreign Key Table Lookup 
        /// </summary>
        public string RefId { get; set; }

        private FieldDefinition refIdFieldDef;
        public FieldDefinition RefIdFieldDef =>
            this.refIdFieldDef ??= this.FieldDef.ModelDef.GetFieldDefinition(this.RefId)
                                   ?? throw new ArgumentException($"Could not find '{this.RefId}' in '{this.RefModel.Name}'");

        /// <summary>
        /// Specify Field to reference (if different from property name)
        /// </summary>
        public string RefField { get; set; }

        private FieldDefinition refFieldDef;
        public FieldDefinition RefFieldDef =>
            this.refFieldDef ??= this.RefModelDef.GetFieldDefinition(this.RefField)
                                 ?? throw new ArgumentException($"Could not find '{this.RefField}' in '{this.RefModelDef.Name}'");
    }
}
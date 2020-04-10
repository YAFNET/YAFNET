/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
CKEDITOR.plugins.add( "syntaxhighlight", {
	requires : "dialog",
	lang : "en,de,fr,zh-cn", // %REMOVE_LINE_CORE%
	init : function( editor ) {
		var command = editor.addCommand( "syntaxhighlightDialog", new CKEDITOR.dialogCommand("syntaxhighlightDialog") );
		command.modes = { wysiwyg: 1, source: 1 };
		
		editor.ui.addButton && editor.ui.addButton( "Syntaxhighlight",
		{
			label : editor.lang.syntaxhighlight.title,
			command : "syntaxhighlightDialog",
			icon: this.path + "icons/code-solid.svg"
		} );

		if ( editor.contextMenu ) {
			editor.addMenuGroup( "syntaxhighlightGroup" );
			editor.addMenuItem( "syntaxhighlightItem", {
				label: editor.lang.syntaxhighlight.contextTitle,
				icon: this.path + "icons/code-solid.svg",
				command: "syntaxhighlightDialog",
				group: "syntaxhighlightGroup"
			});
		
		}

		CKEDITOR.dialog.add( "syntaxhighlightDialog", this.path + "dialogs/syntaxhighlight.js" );
    }
});

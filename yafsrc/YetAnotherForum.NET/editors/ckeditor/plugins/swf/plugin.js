// $Id: plugin.js,v 1.7 2011/01/20 16:04:58 anrikun Exp $

/**
 * @file
 * Written by Henri MEDOT <henri.medot[AT]absyx[DOT]fr>
 * http://www.absyx.fr
 *
 * Portions of code:
 * Copyright (c) 2003-2010, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.html or http://ckeditor.com/license
 */

(function() {

  // Get a CKEDITOR.dialog.contentDefinition object by its ID.
  var getById = function(array, id, recurse) {
    for (var i = 0, item; (item = array[i]); i++) {
      if (item.id == id) return item;
      if (recurse && item[recurse]) {
        var retval = getById(item[recurse], id, recurse);
        if (retval) return retval;
      }
    }
    return null;
  };



  // Override default HTML filter rules to support width/height percentage values.
  var htmlFilterRules = {
    elements: {
      $: function(element) {
        var attributes = element.attributes;
        var realHtml = attributes && attributes._cke_realelement;
        var realFragment = realHtml && new CKEDITOR.htmlParser.fragment.fromHtml(decodeURIComponent(realHtml));
        var realElement = realFragment && realFragment.children[0];

        // If we have width/height in the element, we must move it into the real element.
        if (realElement && element.attributes._cke_resizable) {
          var style = element.attributes.style;
          if (style) {

            // Get width from the style.
            var match;
            match = /(?:^|\s)width\s*:\s*(\d+%?)/i.exec(style);
            var width = match && match[1];

            // Get height from the style.
            match = /(?:^|\s)height\s*:\s*(\d+%?)/i.exec(style);
            var height = match && match[1];

            if (width) {
              realElement.attributes.width = width;
            }

            if (height) {
              realElement.attributes.height = height;
            }
          }
        }

        return realElement;
      }
    }
  };



  // Update the flash preview box.
  var updatePreview = function(dialog) {
    var editor = dialog.getParentEditor();
    dialog.preview = dialog.getElement().getDocument().getById('FlashPreviewBox').getFirst();

    if (!dialog.srcInfo.isValid) {
      dialog.preview.setHtml('');
      dialog.preview.removeAttribute('style');
      return;
    }

    var objectNode = CKEDITOR.dom.element.createFromHtml('<cke:object></cke:object>', editor.document);
    objectNode.setAttributes({classid: 'clsid:d27cdb6e-ae6d-11cf-96b8-444553540000'});

    var embedNode = CKEDITOR.dom.element.createFromHtml('<cke:embed></cke:embed>', editor.document);
    embedNode.setAttributes({type: 'application/x-shockwave-flash'});
    embedNode.appendTo(objectNode);

    var paramMap = {};
    var extraStyles = {};
    var extraAttributes = {};
    dialog.commitContent(objectNode, embedNode, paramMap, extraStyles, extraAttributes);

    dialog.preview.setStyles({
      width: extraStyles.width,
      height: extraStyles.height
    });
    var styles = {width: '100%', height: '100%', display: 'block'};
    objectNode.setStyles(styles);
    embedNode.setStyles(styles);
    dialog.preview.setHtml(objectNode.getOuterHtml().replace(/(<\/?)cke:/gi, '$1'));
  };



  // Return an absolute URL.
  var getAbsoluteUrl = function(dialog, url) {
    var a = dialog.getElement().getDocument().$.createElement('a');
    a.href = url;
    return (CKEDITOR.env.ie7Compat || CKEDITOR.env.ie6Compat) ? a.getAttribute('href', 4) : a.href;
  };



  // Retrieve flashvars from paramMap as key/value pairs.
  var loadFlashvars = function(dialog, paramMap) {
    if (!dialog.loadedFlashvars) {
      dialog.loadedFlashvars = {};
      var value = paramMap.flashvars;
      if (value) {
        var pairs = value.split('&');
        for (var i = 0; i < pairs.length; i++) {
          var pair = pairs[i].split('=');
          if (pair.length == 2) {
            dialog.loadedFlashvars[decodeURIComponent(pair[0])] = decodeURIComponent(pair[1]);
          }
        }
      }
    }
    return dialog.loadedFlashvars;
  };

  var setupFlashvars = function(dialog) {
    if (!dialog._onShowDone) {
      var flashvars = dialog.loadedFlashvars;
      var element = dialog.getContentElement('flashvars', 'flashvarsHtml');
      for (var name in flashvars) {
        addRow(element, name, flashvars[name]);
      }
      addRow(element);
    }
  };



  // load info about the source file and update dialog accordingly.
  var loadInfo = function(dialog) {
    var reset = dialog._onShowDone;

    var src = dialog.srcObj.getValue();
    if (dialog.srcInfo && (dialog.srcInfo.src == src)) return;

    dialog.srcInfo = {src: src, isValid: false};
    if (src) {
      var url;
      if (!reset && dialog.player) {
        src = dialog.loadedFlashvars[dialog.player.fileName]; // A possible player was detected.
        if (dialog.player.jsonName) {
          eval('var obj = ' + dialog.loadedFlashvars[dialog.player.jsonName] + ';');
          src = obj[dialog.player.fileName];
        }
      }
      var url = getAbsoluteUrl(dialog, src);
      $.getJSON(Drupal.settings.ckeditor_swf.getinfo_path, {src: url}, function(data) {
        if (data) {
          dialog.player = null;
          delete dialog.realSrc;
          delete dialog.hiddenFlashvars;

          if (data.mime == 'application/x-shockwave-flash') {
            dialog.srcInfo.isValid = true;
          }
          else if (Drupal.settings.ckeditor_swf.players[data.mime]) {
            dialog.srcInfo.isValid = true;
            dialog.player = Drupal.settings.ckeditor_swf.players[data.mime];
            dialog.realSrc = dialog.player.path;
            dialog.hiddenFlashvars = mergeObjects(dialog.player.hiddenFlashvars);
            dialog.hiddenFlashvars[dialog.player.fileName] = src;
            if (!reset) {
              dialog.srcObj.setValue(src);
              dialog.srcObj.setInitValue();
              delete dialog.loadedFlashvars[dialog.player.fileName];
              if (dialog.player.jsonName) {
                delete dialog.loadedFlashvars[dialog.player.jsonName];
              }
              for (var name in dialog.loadedFlashvars) {
                if (dialog.hiddenFlashvars.propertyIsEnumerable(name)) {
                  dialog.hiddenFlashvars[name] = dialog.loadedFlashvars[name];
                  delete dialog.loadedFlashvars[name];
                }
              }
            }
            if (!data.width || !data.height) {
              data.width = dialog.player.defaultWidth;
              data.height = dialog.player.defaultHeight;
            }
            else if (!reset && dialog.player.offsetHeight) {
              var height = dialog.heightObj.getValue();
              var intRegex = /^\d+$/;
              if (intRegex.test(height)) {
                dialog.heightObj.setValue(Number(height) - dialog.player.offsetHeight);
                dialog.heightObj.setInitValue();
              }
            }
          }
        }
        else if (!reset && dialog.player) {  // A possible player was detected.
          delete dialog.srcInfo;
          delete dialog.player;
          loadInfo(dialog);
          return;
        }

        if (dialog.srcInfo.isValid) {
          dialog.srcInfo.src = src;
          dialog.srcInfo.width = data.width;
          dialog.srcInfo.height = data.height;
          dialog.srcInfo.ratio = data.width / data.height;
          if (reset) {
            dialog.widthObj.setValue(data.width);
            dialog.heightObj.setValue(data.height);
            dialog.btnLock.lock();
          }
          else {
            var ratio = dialog.widthObj.getValue() / dialog.heightObj.getValue();
            dialog.btnLock.lock(isFinite(ratio) && (Math.floor(ratio * 100) == Math.floor(dialog.srcInfo.ratio * 100)));
          }
        }
        else {
          if (reset) {
            dialog.widthObj.setValue('');
            dialog.heightObj.setValue('');
          }
          dialog.btnLock.lock(false);
        }
        setupFlashvars(dialog);
        updatePreview(dialog);
        dialog._onShowDone = true;
      });
    }
    else {
      delete dialog.player;
      setupFlashvars(dialog);
      updatePreview(dialog);
      dialog.btnLock.lock(false);
      dialog._onShowDone = true;
    }
  };



  var addRow = function(element, name, value) {
    if (!element.tbody) {
      element.tbody = $('tbody', element.getElement().$);
    }
    var tbody = element.tbody;
    tbody.append('<tr><td class="cke_dialog_ui_vbox_child cke_dialog_ui_hbox_first">'
      + '<div class="cke_dialog_ui_input_text"><input type="text" name="varName" value="" class="cke_dialog_ui_input_text" /></div></td>'
      + '<td class="cke_dialog_ui_vbox_child cke_dialog_ui_hbox_first">'
      + '<div class="cke_dialog_ui_input_text"><input type="text" name="varValue" value="" class="cke_dialog_ui_input_text" /></div></td>'
      + '<td class="cke_dialog_ui_vbox_child vbox_child cke_dialog_ui_hbox_last">'
      + '<a class="cke_dialog_ui_button remove" href="javascript:void(0)"><span class="cke_dialog_ui_button">\u2715</span></a></td></tr>'
    );
    var row = $('tr:last', tbody);
    var inputs = $('input', row);
    var a = $('a.remove', row);
    a.attr('title', element.getDialog().getParentEditor().lang.select.btnDelete);

    if (name) {
      inputs.eq(0).val(name);
      inputs.eq(1).val((value) ? value : '');
    }
    else {
      a.hide();
    }

    inputs.change(function() {
      if (inputs.val() == '') {
        if (row.next().length > 0) {
          row.remove();
        }
      }
      else if (row.next().length == 0) {
        a.show();
        addRow(element);
      }
      element._changed = true;
    }).keyup(function() {
      element._changed = true;
    });
    a.click(function() {
      row.remove();
      element._changed = true;
    });
  };



  var getParamByName = function(objectNode, name) {
    //var params = objectNode.getElementsByTag('param', 'cke'); //removed because IE does not seem to detect cke:param tags
    var children = objectNode.getChildren();
    for (var i = 0; i < children.count(); i++) {
      var child = children.getItem(i);
      if ((child.$.nodeType == 1) && (child.getName() == 'cke:param') && (child.getAttribute('name') == name)) { //getName() crashes on Safari 4 when used on non-element nodes
        return child;
      }
    }
    return null;
  };



  var getChildrenByTagName = function(parent, tagName) {
    var children = parent.getChildren();
    var result = [];
    for (var i = 0; i < children.count(); i++) {
      var child = children.getItem(i);
      if ((child.$.nodeType == 1) && (child.getName() == tagName)) {
        result.push(child);
      }
    }
    return result;
  };



  var commitParam = function(objectNode, embedNode, name, value) {
    var param = getParamByName(objectNode, name);
    if (value) {
      if (param) {
        param.setAttribute('value', value);
      }
      else {
        param = CKEDITOR.dom.element.createFromHtml('<cke:param></cke:param>', objectNode.getDocument());
        param.setAttributes({name: name, value: value});
        if (objectNode.getChildCount() < 1) {
          param.appendTo(objectNode);
        }
        else {
          param.insertBefore(objectNode.getFirst());
        }
      }
      if (embedNode) {
        embedNode.setAttribute(name, value);
      }
    }
    else {
      if (param) {
        param.remove();
      }
      if (embedNode) {
        embedNode.removeAttribute(name);
      }
    }
  };



  var mergeObjects = function() {
    var obj = {};
    for (var i = 0; i < arguments.length; i++) {
      var arg = arguments[i];
      if (typeof arg == 'object') {
        for (var name in arg) {
          obj[name] = arg[name];
        }
      }
    }
    return obj;
  };



  CKEDITOR.plugins.add('swf', {

    init: function(editor, pluginPath) {
      CKEDITOR.on('dialogDefinition', function(e) {
        if ((e.editor != editor) || (e.data.name != 'flash')) return;

        // Overrides definition.
        var definition = e.data.definition;
        var infoTab = definition.getContents('info');

        definition.onLoad = function() {
          this.selectPage = CKEDITOR.tools.override(this.selectPage, function(original) {
            return function(id) {
              var oldId = this._.currentTabId;
              original.call(this, id);
              if ((this._.currentTabId == 'info') && (this._.currentTabId != oldId)) {
                updatePreview(this);
              }
            };
          });
        };

        definition.onShow = function() {
          this._onShowDone = false;

          // Clear previously saved elements.
          this.fakeImage = null;
          this.objectNode = null;
          this.embedNode = null;
          delete this.srcInfo;
          delete this.loadedFlashvars;
          delete this.player;
          delete this.realSrc;
          delete this.hiddenFlashvars;

          // Try to detect any embed or object tag that has Flash parameters.
          var fakeImage = this.getSelectedElement();
          if (fakeImage
           && (fakeImage.data && fakeImage.data('cke-real-element-type') && fakeImage.data('cke-real-element-type') == 'flash'
            || fakeImage.getAttribute('_cke_real_element_type') && fakeImage.getAttribute('_cke_real_element_type') == 'flash')) {
            this.fakeImage = fakeImage;
            var realElement = editor.restoreRealElement(fakeImage);
            var objectNode = null;
            var embedNode = null;
            var paramMap = {};
            if (realElement.getName() == 'cke:object') {
              objectNode = realElement;
              var embeds = getChildrenByTagName(objectNode, 'cke:embed');
              if (embeds.length > 0) {
                embedNode = embeds[0];
              }
              var params = getChildrenByTagName(objectNode, 'cke:param');
              for (var i = 0; i < params.length; i++) {
                var param = params[i];
                paramMap[param.getAttribute('name')] = param.getAttribute('value');
              }
            }
            else if (realElement.getName() == 'cke:embed') {
              embedNode = realElement;
            }

            this.objectNode = objectNode;
            this.embedNode = embedNode;

            this.setupContent(objectNode, embedNode, paramMap, fakeImage);

            // Retrieve flashvars.
            var flashvars = loadFlashvars(this, paramMap);

            // Detect a possible player.
            var url = getAbsoluteUrl(this, this.srcObj.getValue());
            var players = Drupal.settings.ckeditor_swf.players;
            if (players) {
              var player;
              var found = false;
              for (var mime in players) {
                player = players[mime];
                var path = getAbsoluteUrl(this, player.path);
                if (url == path) {
                  found = true;
                  break;
                }
              }
              if (found && (flashvars[player.fileName] || player.jsonName && flashvars[player.jsonName])) {
                this.player = player;
              }
            }
          }

          // Init srcInfo.
          loadInfo(this);
        };

        definition.onOk = function() {

          // If there's no selected object or embed, create one.
          // Otherwise, reuse the selected object and embed nodes.
          var objectNode = null;
          var embedNode = null;
          var paramMap = {};
          var extraStyles = {};
          var extraAttributes = {};

          if (!this.fakeImage) {
            objectNode = CKEDITOR.dom.element.createFromHtml('<cke:object></cke:object>', editor.document);
            objectNode.setAttribute('classid', 'clsid:d27cdb6e-ae6d-11cf-96b8-444553540000');
            if (editor.config.flashAddEmbedTag) {
              embedNode = CKEDITOR.dom.element.createFromHtml('<cke:embed></cke:embed>', editor.document);
              embedNode.setAttribute('type', 'application/x-shockwave-flash');
              embedNode.appendTo(objectNode);
            }
          }
          else {
            objectNode = this.objectNode;
            embedNode = this.embedNode;
          }

          // Produce the paramMap.
          var params = getChildrenByTagName(objectNode, 'cke:param');
          for (var i = 0; i < params.length; i++) {
            var param = params[i];
            paramMap[param.getAttribute('name')] = param;
          }

          // Apply or remove flash parameters.
          this.commitContent(objectNode, embedNode, paramMap, extraStyles, extraAttributes);

          // Refresh the fake image.
          var newFakeImage = editor.createFakeElement(objectNode || embedNode, 'cke_flash', 'flash', true);
          newFakeImage.setAttributes(extraAttributes);
          newFakeImage.setStyles(extraStyles);
          if (this.fakeImage) {
            newFakeImage.replace(this.fakeImage);
            editor.getSelection().selectElement(newFakeImage);
          }
          else {
            editor.insertElement(newFakeImage);
          }
        };

        // Overrides src definition.
        var content;
        content = getById(infoTab.elements, 'src', 'children');
        content.commit = CKEDITOR.tools.override(content.commit, function(original) {
          return function(objectNode, embedNode, paramMap, fakeImage) {
            var dialog = this.getDialog();
            if (dialog.player) {
              commitParam(objectNode, null, 'movie', dialog.realSrc);
              if (embedNode) {
                embedNode.setAttribute('src', dialog.realSrc);
              }
            }
            else {
              original.call(this, objectNode, embedNode, paramMap, fakeImage);
            }
          };
        });
        content.onLoad = function() {
          this.getDialog().srcObj = this;
        };
        content.onChange = function() {
          var dialog = this.getDialog();
          if (dialog._onShowDone) {
            loadInfo(dialog);
          }
        };

        // Overrides width definition.
        content = getById(infoTab.elements, 'width', 'children');
        var intRegex = /^\d+$/;
        var dimRegex = /^[1-9]\d*%?$/;
        content.validate = CKEDITOR.dialog.validate.regex(dimRegex, editor.lang.flash.validateWidth),
        content.onLoad = function() {
          var dialog = this.getDialog();
          dialog.widthObj = this;
          this.getInputElement().on('change', function(e) {
            if (dialog.srcInfo.isValid && dialog.btnLock.isLocked()) {
              var value = this.getValue();
              if (value && intRegex.test(value)) {
                dialog.heightObj.setValue(Math.round(value / dialog.srcInfo.ratio));
              }
              else {
                dialog.btnLock.lock(false);
              }
            }
            updatePreview(dialog);
          }, this);
        };
        content.setup = CKEDITOR.tools.override(content.setup, function(original) {
          return function(objectNode, embedNode, paramMap, fakeImage) {
            original.call(this, objectNode, embedNode, paramMap, fakeImage);
            if (fakeImage) {
              var width = fakeImage.$.style.width;
              if (dimRegex.test(width)) {
                this.setValue(width);
              }
            }
          };
        });
        content.commit = CKEDITOR.tools.override(content.commit, function(original) {
          return function(objectNode, embedNode, paramMap, extraStyles) {
            original.call(this, objectNode, embedNode, paramMap, extraStyles);
            var value = this.getValue();
            if (intRegex.test(value)) {
              value += 'px';
            }
            extraStyles.width = value;
          };
        });

        // Overrides height definition.
        content = getById(infoTab.elements, 'height', 'children');
        content.validate = CKEDITOR.dialog.validate.regex(dimRegex, editor.lang.flash.validateHeight),
        content.onLoad = function() {
          var dialog = this.getDialog();
          dialog.heightObj = this;
          this.getInputElement().on('change', function(e) {
            if (dialog.srcInfo.isValid && dialog.btnLock.isLocked()) {
              var value = this.getValue();
              if (value && intRegex.test(value)) {
                dialog.widthObj.setValue(Math.round(value * dialog.srcInfo.ratio));
              }
              else {
                dialog.btnLock.lock(false);
              }
            }
            updatePreview(dialog);
          }, this);
        };
        content.setup = CKEDITOR.tools.override(content.setup, function(original) {
          return function(objectNode, embedNode, paramMap, fakeImage) {
            original.call(this, objectNode, embedNode, paramMap, fakeImage);
            if (fakeImage) {
              var height = fakeImage.$.style.height;
              if (dimRegex.test(height)) {
                this.setValue(height);
              }
            }
          };
        });
        content.commit = CKEDITOR.tools.override(content.commit, function(original) {
          return function(objectNode, embedNode, paramMap, extraStyles) {
            original.call(this, objectNode, embedNode, paramMap, extraStyles);
            var value = this.getValue();
            if (intRegex.test(value)) {
              var player = this.getDialog().player;
              if (player && player.offsetHeight) {
                value = Number(value) + player.offsetHeight;
              }
              value += 'px';
            }
            extraStyles.height = value;
          };
        });

        // Remove hSpace and vSpace definitions
        // Replace them by lock and reset buttons definitions
        var element = definition.getContents('info').elements[1];
        element.widths = ['25%', '25%', '50%'];
        element.children.pop();
        element.children[2] = {
          type: 'html',
          style : 'margin-top:14px;width:36px;',
          html: '<div><a href="javascript:void(0)" tabindex="-1" title="' + editor.lang.image.lockRatio + '" class="cke_btn_locked"></a>'
            + '<a href="javascript:void(0)" tabindex="-1" title="' + editor.lang.image.resetSize + '" class="cke_btn_reset"></a></div>',
          onLoad: function() {
            var buttons = $('a', this.getElement().$);
            buttons.mouseover(function() {
              $(this).addClass('cke_btn_over');
            }).mouseout(function() {
              $(this).removeClass('cke_btn_over');
            });

            var dialog = this.getDialog();
            dialog.btnLock = buttons.eq(0);
            dialog.btnLock.isLocked = function() {
              return !this.hasClass('cke_btn_unlocked');
            };
            dialog.btnLock.lock = function(locked) {
              if ((locked == null) || locked) {
                this.removeClass('cke_btn_unlocked');
              }
              else {
                this.addClass('cke_btn_unlocked');
              }
            };
            dialog.btnLock.click(function() {
              if (!dialog.btnLock.isLocked() && dialog.srcInfo.isValid) {
                var canLock = false;
                var value = dialog.widthObj.getValue();
                if (value && intRegex.test(value)) {
                  dialog.heightObj.setValue(Math.round(value / dialog.srcInfo.ratio));
                  canLock = true;
                }
                else {
                  value = dialog.heightObj.getValue();
                  if (value && intRegex.test(value)) {
                    dialog.widthObj.setValue(Math.round(value * dialog.srcInfo.ratio));
                    canLock = true;
                  }
                }
                if (canLock) {
                  dialog.btnLock.lock();
                  updatePreview(dialog);
                }
              }
              else {
                dialog.btnLock.lock(false);
              }
            });

            buttons.eq(1).click(function() {
              if (dialog.srcInfo.isValid) {
                dialog.widthObj.setValue(dialog.srcInfo.width);
                dialog.heightObj.setValue(dialog.srcInfo.height);
                dialog.btnLock.lock();
                updatePreview(dialog);
              }
            });
          }
        };

        // Overrides preview definition.
        content = getById(infoTab.elements, 'preview', 'children');
        content.html = '<div>' + CKEDITOR.tools.htmlEncode(editor.lang.preview) + '<br />'
          + '<div id="FlashPreviewBox" style="padding:0;"><div></div></div></div>';

        // Add base param textfield
        var propertiesTab = definition.getContents('properties');
        propertiesTab.elements[2].children[1] = {
          id: 'base',
          type: 'text',
          label: 'Base',
          setup: function(objectNode, embedNode, paramMap, fakeImage) {
            var param = getParamByName(objectNode, 'base');
            if (param) {
              this.setValue(param.getAttribute('value'));
            }
          },
          commit: function(objectNode, embedNode, paramMap, extraStyles) {
            commitParam(objectNode, embedNode, 'base', this.getValue());
          }
        };

        // Overrides allowFullScreen definition to fix flash plugin's bug.
        content = getById(propertiesTab.elements, 'allowFullScreen', 'children');
        content.setup = function(objectNode, embedNode, paramMap, fakeImage) {
          var param = getParamByName(objectNode, 'allowFullScreen');
          this.setValue((param) ? (param.getAttribute('value').toLowerCase() == 'true') : false);
        };
        content.commit = function(objectNode, embedNode, paramMap, extraStyles) {
          commitParam(objectNode, embedNode, 'allowFullScreen', this.getValue());
        };

        // Add Flashvars tab.
        definition.contents.push({
          id: 'flashvars',
          label: 'Flashvars',
          accessKey: 'F',
          elements: [{
            id: 'flashvarsHtml',
            type: 'html',
            html: '<table style="width:100%;"><thead><tr>'
              + '<th style="width:20%;">' + editor.lang.textfield.name + '</th>'
              + '<th style="width:75%;">' + editor.lang.textfield.value + '</th>'
              + '<th style="width: 5%;">&nbsp;</th></tr></thead>'
              + '<tbody></tbody></table>',
            setup: function(objectNode, embedNode, paramMap, fakeImage) {
              this._changed = false;
            },
            commit: function(objectNode, embedNode, paramMap, extraStyles) {
              var flashvars = {};
              var inputs = $('input', this.tbody);
              for (var i = 0; i < inputs.length; i = i + 2) {
                if (inputs.eq(i).val()) {
                  flashvars[inputs.eq(i).val()] = inputs.eq(i + 1).val();
                }
              }
              var dialog = this.getDialog();
              if (dialog.hiddenFlashvars && dialog.player) {
                delete flashvars[dialog.player.fileName];
                flashvars = mergeObjects(dialog.hiddenFlashvars, flashvars);
                if (dialog.player.jsonName) {
                  flashvars = {};
                  flashvars[dialog.player.jsonName] = '{"' + dialog.player.fileName + '":"' + dialog.hiddenFlashvars[dialog.player.fileName] + '"}';
                }
              }
              var pairs = [];
              for (var name in flashvars) {
                if (flashvars[name] != null) {
                  pairs.push(encodeURIComponent(name) + '=' + encodeURIComponent(flashvars[name]));
                }
              }
              commitParam(objectNode, embedNode, 'flashvars', pairs.join('&'));
            },
            onLoad: function() {
              this.isChanged = function() {
                return this._changed;
              };
            },
            onHide: function() {
              this.tbody.html('');
            }
          }]
        });
      });
    },

    requires: ['htmlwriter'],
    afterInit: function(editor) {
      var dataProcessor = editor.dataProcessor;
      var htmlFilter = dataProcessor && dataProcessor.htmlFilter;
      if (htmlFilter) {
        htmlFilter.addRules(htmlFilterRules, 5);
      }
    }
  });
})();
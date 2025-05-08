class RulesComponent extends HTMLElement {
    constructor() {
        super();
        this.rule = null;
        this.bod = null;
        this.groupName = null;
        this.sMode = null;
        this.rowAttributes = null;
        this.ruleObj = [];
    }
    static get observedAttributes() {
        return ['rule', 'bod', 'groupname', "mode"];
    }

    attributeChangedCallback(property, oldValue, newValue) {

        if (oldValue === newValue) return;
        this[property] = newValue;

    }

    connectedCallback() {
        this.rule = JSON.parse(this.rule);
        this.bod = JSON.parse(this.bod);
        this.groupName = this.groupname;
        this.sMode = this.mode;
        this.rowAttributes = this.bod.Groups[this.groupName.toLowerCase()].BOFieldNames.split(",").map(x => x.trim());

        const style = document.createElement('style');
        style.textContent = `@import url('https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css');`;

        const shadow = this.attachShadow({ mode: 'open' });
        shadow.appendChild(style);
        this.buildRuleTree(shadow, this.rule, true);
        var saveButton = document.createElement('button');
        saveButton.textContent = "Save";
        saveButton.addEventListener('click', () => { this.ruleObj = this.saveRules(shadow, []); this.emitRule(); });
        shadow.appendChild(saveButton);

    }

    emitRule() {
        debugger
        const event = new CustomEvent('emitRule', {
            detail: {
                value: this.ruleObj,
                mode: this.sMode
            },
            bubbles: true,
            composed: true,
        });
        this.dispatchEvent(event);
    }

    buildRuleTree(shadow, rule, isFirstCall) {
        rule?.forEach((r, idx) => {
            if (r.sRuleOperator !== null && r.sRuleOperator !== '') {
                this.addGroup(shadow, r, isFirstCall);
            }
            else {
                this.addRule(shadow, r);
            }

        });
    }

    saveRules(shadow, ruleObj) {
        debugger
        var ruleHeader = shadow.querySelector('#rule-header');
        var ruleBody = shadow.querySelector('#rule-body');
        var leafRuleNodes = Array.from(ruleBody.children).filter(c => c.classList.contains('rule-row'));
        var parentRuleNodes = Array.from(ruleBody.children).filter(c => c.classList.contains('rule-group-body'));

        var hiddenAttributes = Array.from(ruleBody.children).filter(c => c.offsetParent === null && c.hasAttribute('data-attrname')).map(h => {
            var attrName = h.getAttribute('data-attrname');
            return {
                attrName: attrName,
                value: h.value,
                hidden: true,
            };
        });

        var parentRules = [];
        if (parentRuleNodes.length > 0) {
            parentRuleNodes.forEach(parent => {
                parentRules = parentRules.concat(this.saveRules(parent, []));
            });
        }
        var leafRules = leafRuleNodes.map(lr => Array.from(lr.children).filter(c => c.getAttribute('data-attrname') !== null).map(c => {
            debugger
            var attrName = c.getAttribute('data-attrname');
            var rowid = lr.getAttribute('data-id');
            var key = c.getAttribute('data-key');
            var hidden = c.getAttribute('data-hidden') === "true";
            return {
                attrName: rowid + '_' + attrName,
                value: c.value,
                key: key,
                hidden: hidden,
            };
        }));

        ruleObj.push({
            sRuleOperator: ruleHeader.querySelector('#rulegroup-operator').value,
            leafRules: leafRules,
            parentRules: parentRules,
            hiddenAttributes: hiddenAttributes,
        });
        return ruleObj;
    }

    addRule(ruleBody, ruleContent) {
        const rowTemplate = document.getElementById('row-template').content.cloneNode(true);
        const rowDiv = rowTemplate.getElementById('row');
        const rowid = this.generateRandomString(32);
        rowDiv.setAttribute('data-id', rowid);
        this.rowAttributes.forEach((att) => {
            var attProp = this.bod.Attributes[att.toLowerCase()];
            if (attProp) {
                var control = document.createElement('div');
                if (attProp.FKiType > 0) {
                    const selectTemplate = document.getElementById('select-template').content.cloneNode(true);
                    control = selectTemplate.getElementById("select-control");
                    control.setAttribute('name', attProp["LabelName"]);
                    control.setAttribute('data-attrname', attProp["Name"]);
                    /*if ((attProp.FKiType === 10 || attProp.FKiType === 40) && attProp.FieldDDL?.length > 0) {*/
                    if (attProp.FKiType === 10 || attProp.FKiType === 40) {
                        control.add(new Option("Please select", "", true));
                        attProp.FieldDDL?.forEach(opt => {
                            /*control.add(new Option(opt.Expression, opt.text, null, opt.text === (ruleContent ? ruleContent[attProp["Name"]] : null)));*/
                            control.add(new Option(opt.Expression, opt.text));
                        });
                    }
                    else if (attProp.IsOptionList) {
                        if (attProp.OptionList !== null && attProp.OptionList.length > 0) {
                            control.add(new Option("Please select", "", attProp.DefaultValue ?? true));
                            attProp.OptionList.forEach(opt => {
                                control.add(new Option(opt.OptionName, opt.sValues, opt.OptionName === attProp.DefaultValue));
                            });
                        }
                    }
                    /*control.value = ruleContent ? ruleContent[attProp["Name"]] : "";*/
                    //else if (attProp.iOneClickID > 0 || (attProp.iOneClickIDXIGUID !== null && attProp.iOneClickIDXIGUID != '00000000-0000-0000-0000-000000000000')) {

                    //}
                }
                else if (attProp["Name"] === 'FKiOperator') {
                    const selectTemplate = document.getElementById('select-template').content.cloneNode(true);
                    control = selectTemplate.getElementById("select-control");
                    control.setAttribute('name', attProp["LabelName"]);
                    control.setAttribute('data-attrname', attProp["Name"]);
                    control.add(new Option("Please select", "", true));
                    control.add(new Option("equals", "="));
                    control.add(new Option("greater than", ">"));
                    control.add(new Option("less than", "<"));
                    control.add(new Option("greater than or equal to", ">="));
                    control.add(new Option("less than or equal to", "<="));
                    control.add(new Option("not equal to", "!="));
                }
                else {
                    debugger
                    control = document.createElement('input');
                    control.setAttribute("name", attProp["LabelName"]);
                    control.setAttribute('data-attrname', attProp["Name"]);
                    control.value = ruleContent ? ruleContent[attProp["Name"]] : attProp["Name"] === "XIDeleted" ? "0" : "";
                    if (attProp.bIsHidden) {
                        control.hidden = true;
                        control.setAttribute('data-hidden', 'true');
                    }
                }
                if (attProp.sEventHandler) {
                    var sHandler = attProp.sEventHandler.split('=').pop();
                    var sDepBOFieldIDs = attProp.sDepBOFieldIDs.split(',');
                    var sDepFieldNames = sDepBOFieldIDs.map(id => this.bod.Attributes[Object.keys(this.bod.Attributes).find(k => this.bod.Attributes[k].ID === Number(id))].Name).join(",");

                    if (attProp.ID === 1000864) {
                        var sOneClick = sHandler.split('^').pop();
                        sOneClick = sOneClick.substring(1, sOneClick.length - 2);
                        control.addEventListener('change', (e) => {
                            control.setAttribute('data-key', e.target.options[e.target.selectedIndex].text);
                            this.fncGetDependencyBOAttributesMultiRow(e, sOneClick, sDepFieldNames, rowid, ruleContent ? ruleContent[sDepFieldNames.split(",")[0]] : null);
                        });

                    }
                    else if (attProp.ID === 38627 || attProp.ID === 1000865) {
                        control.addEventListener('change', (e) => {
                            control.setAttribute('data-key', e.target.options[e.target.selectedIndex].text);
                            this.fncGetAttributeWhereValues(e, sDepFieldNames, rowid, ruleContent ? ruleContent[sDepFieldNames.split(",")[0]] : null, ruleContent ? ruleContent[sDepFieldNames.split(",").pop()] : null, this.sMode);
                        });

                    }
                    else if (attProp.ID === 38659) {
                        control.addEventListener('change', (e) => {
                            control.setAttribute('data-key', e.target.options[e.target.selectedIndex].text);
                            this.fncGetAttributeRangeValues(e, this.sMode === "object" ? "BOAttributeIDXIGUID" : "QSFieldOriginIDXIGUID", sDepFieldNames, rowid, ruleContent ? ruleContent[sDepFieldNames.split(",")[0]] : null);
                        });
                    }
                }
                rowDiv.appendChild(control);
                control.value = ruleContent ? ruleContent[attProp["Name"]] : attProp["Name"] === "XIDeleted" ? "0" : "";
                if (control.value) {
                    control.dispatchEvent(new Event('change'));
                }
            }
        });

        const deleteButtonTemplate = document.getElementById('button-template').content.cloneNode(true);
        const deleteButton = deleteButtonTemplate.getElementById('delete-button');
        deleteButton.addEventListener('click', () => { this.deleteRule(rowDiv); });
        rowDiv.appendChild(deleteButton);
        ruleBody.appendChild(rowDiv);
    }

    addGroup(ruleBody, ruleContent, isFirstCall = false) {
        const template = document.getElementById('rule-template').content.cloneNode(true);
        var newRuleGroupBody = template.getElementById("rule-template-body");
        var newRuleBody = template.getElementById("rule-body");
        if (isFirstCall) {
            var deleteButton = template.getElementById("delete-button");
            deleteButton.remove();
        }

        var newid = this.generateRandomString(32);
        newRuleGroupBody.setAttribute('data-id', newid);

        var newRuleGroupOp = template.getElementById('rulegroup-operator');
        newRuleGroupOp.value = ruleContent ? ruleContent.sRuleOperator : "and";

        var hiddenAttributes = Object.keys(this.bod.Attributes).filter(k => this.bod.Attributes[k]["bIsHidden"]);
        hiddenAttributes.forEach(ha => {
            var control = document.createElement('input');
            var attProp = this.bod.Attributes[ha];
            control.setAttribute("name", attProp["LabelName"]);
            control.setAttribute('data-attrname', newRuleGroupBody.getAttribute('data-id') + "_" + attProp["Name"]);
            control.value = ruleContent ? ruleContent[attProp["Name"]] : "";
            control.hidden = true;
            newRuleBody.appendChild(control);
        });

        template.getElementById('addrule-button').addEventListener('click', () => { this.addRule(newRuleBody); });
        template.getElementById('addgroup-button').addEventListener('click', () => { this.addGroup(newRuleBody); });
        if (!isFirstCall) {
            template.getElementById('delete-button').addEventListener('click', () => { this.deleteGroup(newRuleBody); });
        }
        ruleBody.appendChild(template);
        if (ruleContent) {
            this.buildRuleTree(newRuleBody, ruleContent.oChildGroups, false);
        }

    }

    deleteGroup(ruleBody) {
        console.log('delete group: ', ruleBody);
        var del = Array.from(ruleBody.children).find(c => c.hasAttribute('data-attrname') && c.getAttribute('data-attrname').split("_").pop() === "XIDeleted");
        del.value = '1';
        ruleBody.parentElement.style.display = 'none';
    }

    deleteRule(ruleBody) {
        console.log('delete rule: ', ruleBody);
        var del = ruleBody.querySelector('[data-attrname="XIDeleted"]');
        console.log('del: ', del, del.value);
        del.value = '1';
        console.log('del after: ', del, del.value);
        ruleBody.style.display = 'none';
    }

    generateRandomString(length) {
        const characters = '-abcdefghijklmnopqrstuvwxyz0123456789';
        let result = '';
        const charactersLength = characters.length;
        for (let i = 0; i < length; i++) {
            result += characters.charAt(Math.floor(Math.random() * charactersLength));
        }
        return result;
    }

    fncGetDependencyBOAttributesMultiRow(event, i1ClickID, sField, sRowIdentifier, currentValue) {
        console.log('currentValue: ', currentValue);
        var obj = [];
        var oNVParams = new Object();
        oNVParams.sName = '{XIP|BOIDXIGUID}';
        oNVParams.sValue = event.target.value;
        obj.push(oNVParams);
        $.ajax({
            type: 'POST',
            url: GetDependentFieldValuesURL,
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ i1ClickID: i1ClickID, sGUID: null, oNVParams: obj }),
            cache: false,
            async: true,
            dataType: 'json',
            success: function (data) {
                const shadowRoot = document.querySelector('rules-component').shadowRoot;
                var rowObj = shadowRoot.querySelector("[data-id='" + sRowIdentifier + "']");
                var selectObj = rowObj.querySelector("[data-attrname='" + sField + "']");
                if (!currentValue) {
                    selectObj.add(new Option("Please select", "", null, true));
                }
                else {
                    selectObj.add(new Option("Please select", ""));
                }

                data.forEach(x => {
                    selectObj.add(new Option(x.name.sValue, x.xiguid.sValue, null, x.xiguid.sValue === currentValue));
                })
                if (currentValue) {
                    selectObj.dispatchEvent(new Event('change'));
                }
            },
            error: function (data) {
            }
        })

    }
    fncGetAttributeWhereValues(event, sFieldNames, sRowIdentifier, currentValue1, currentValue2, sMode) {
        debugger;
        console.log(event, sFieldNames, sRowIdentifier, currentValue1, currentValue2);
        var depFields = sFieldNames.split(",");
        var sField = depFields[0];
        var sOperatorField = depFields[1];
        $.ajax({
            type: 'POST',
            url: LoadWhereValuesURL,
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ sAttributeIDXIGUID: event.target.value, sType: sMode }),
            cache: false,
            async: true,
            dataType: 'json',
            success: function (data) {
                debugger
                console.log('data', data);
                if (data != '') {
                    var attributedata = data;
                    const shadowRoot = document.querySelector('rules-component').shadowRoot;
                    var rowObj = shadowRoot.querySelector("[data-id='" + sRowIdentifier + "']");
                    var depFieldObj = rowObj.querySelector("[data-attrname='" + sField + "']");
                    var depOpFieldObj = rowObj.querySelector("[data-attrname='" + sOperatorField + "']");

                    if (attributedata.IsOptionList || attributedata.bIsOptionList) {
                        const selectTemplate = document.getElementById('select-template').content.cloneNode(true);
                        var selectObj = selectTemplate.getElementById("select-control");
                        selectObj.setAttribute('name', depFieldObj.getAttribute('name'));
                        selectObj.setAttribute('data-attrname', depFieldObj.getAttribute('data-attrname'));
                        selectObj.setAttribute('data-typeId', "options");
                        if (!currentValue1) {
                            selectObj.add(new Option("Please select", "", null, true));
                        }
                        else {
                            selectObj.add(new Option("Please select", ""));
                        }

                        if (attributedata.OptionList) {
                            attributedata.OptionList.forEach(x => {
                                selectObj.add(new Option(x.sOptionName, x.sValues, null, x.sValues === currentValue1));
                            });
                        }
                        else if (attributedata.FieldOptionList) {
                            attributedata.FieldOptionList.forEach(x => {
                                selectObj.add(new Option(x.sOptionName, x.sOptionValue, null, x.sOptionValue === currentValue1));
                            });
                        }
                        var parent = depFieldObj.parentNode;
                        parent.replaceChild(selectObj, depFieldObj);

                        while (depOpFieldObj.options.length > 0) {
                            depOpFieldObj.remove(0);
                        }
                        depOpFieldObj.add(new Option("Please select", "", currentValue2 ?? true));
                        depOpFieldObj.add(new Option("equals", "=", null, currentValue2 === '='));
                        depOpFieldObj.add(new Option("greater than", ">", null, currentValue2 === '>'));
                        depOpFieldObj.add(new Option("less than", "<", null, currentValue2 === '<'));
                        depOpFieldObj.add(new Option("greater than or equal to", ">=", null, currentValue2 === '>='));
                        depOpFieldObj.add(new Option("less than or equal to", "<=", null, currentValue2 === '<='));
                        depOpFieldObj.add(new Option("not equal to", "!=", null, currentValue2 === '!='));
                    }
                    else if (attributedata.TypeID === 150 || attributedata.TypeID === 110 || attributedata.sBaseDataType === "datetime") {
                        const dateElement = document.createElement('input')
                        dateElement.setAttribute('name', depFieldObj.getAttribute('name'));
                        dateElement.setAttribute('data-attrname', depFieldObj.getAttribute('data-attrname'));
                        dateElement.setAttribute('data-typeId', attributedata.TypeID ?? attributedata.sBaseDataType);
                        if (attributedata.TypeID === 110) {
                            dateElement.type = 'date';
                        }
                        else if (attributedata.TypeID === 150 || attributedata.sBaseDataType === "datetime") {
                            dateElement.type = 'datetime-local';
                        }
                        var parent = depFieldObj.parentNode;
                        parent.replaceChild(dateElement, depFieldObj);

                        while (depOpFieldObj.options.length > 0) {
                            depOpFieldObj.remove(0);
                        }
                        depOpFieldObj.add(new Option("Please select", "", currentValue2 ?? true));
                        depOpFieldObj.add(new Option("equals", "=", null, currentValue2 === '='));
                        depOpFieldObj.add(new Option("greater than", ">", null, currentValue2 === '>'));
                        depOpFieldObj.add(new Option("less than", "<", null, currentValue2 === '<'));
                        depOpFieldObj.add(new Option("greater than or equal to", ">=", null, currentValue2 === '>='));
                        depOpFieldObj.add(new Option("less than or equal to", "<=", null, currentValue2 === '<='));
                        depOpFieldObj.add(new Option("not equal to", "!=", null, currentValue2 === '!='));
                        depOpFieldObj.add(new Option("between", "between", null, currentValue2 === 'between'));
                        depOpFieldObj.add(new Option("range", "range", null, currentValue2 === 'range'));

                        if (currentValue2 === "range") {
                            depOpFieldObj.dispatchEvent(new Event("change"));
                        }

                    }
                    else if (attributedata.FKTableName && attributedata.FKTableName.length > 0) {
                        var iBODID = 0;
                        var sBOName = attributedata.FKTableName
                        var AutoCompleteList = 0;
                        var sRefType = "bo";
                        if (attributedata.iOneClickID > 0) {
                            sRefType = "1click";
                            iBODID = attributedata.iOneClickID;
                        }
                        $.ajax({
                            url: GetAutoCompleteList,
                            type: 'POST',
                            contentType: "application/json; charset=utf-8",
                            datatype: "json",
                            async: true,
                            cache: false,
                            data: JSON.stringify({ sType: sRefType, iBODID: iBODID, sBOName: sBOName, sGUID: '@sGUID' }),
                            success: function (data) {
                                const selectTemplate = document.getElementById('select-template').content.cloneNode(true);
                                var selectObj = selectTemplate.getElementById("select-control");
                                selectObj.setAttribute('name', depFieldObj.getAttribute('name'));
                                selectObj.setAttribute('data-attrname', depFieldObj.getAttribute('data-attrname'));
                                if (!currentValue) {
                                    selectObj.add(new Option("Please select", "", null, true));
                                }
                                else {
                                    selectObj.add(new Option("Please select", ""));
                                }
                                //attributedata.OptionList.forEach(x => {
                                //    selectObj.add(new Option(x.sOptionName, x.sValues));
                                //})
                                var parent = depFieldObj.parentNode;
                                parent.replaceChild(selectObj, depFieldObj);
                            }
                        });
                    }
                    else {
                        if (attributedata.TypeID === 20 || attributedata.sBaseDataType === "boolean") {
                            var checkBoxElement = document.createElement('input');
                            checkBoxElement.type = "checkbox";
                            checkBoxElement.setAttribute('name', depFieldObj.getAttribute('name'));
                            checkBoxElement.setAttribute('data-attrname', depFieldObj.getAttribute('data-attrname'));
                            var parentNode = depFieldObj.parentNode;
                            parentNode.replaceChild(checkBoxElement, depFieldObj);

                            while (depOpFieldObj.options.length > 0) {
                                depOpFieldObj.remove(0);
                            }
                            depOpFieldObj.add(new Option("equals", "="));

                        }
                        else if (attributedata.TypeID === 180 || attributedata.sBaseDataType === "varchar" || attributedata.sBaseDataType === "nvarchar") {
                            while (depOpFieldObj.options.length > 0) {
                                depOpFieldObj.remove(0);
                            }
                            depOpFieldObj.add(new Option("Please select", "", true));
                            depOpFieldObj.add(new Option("starts with", "starts with", null, currentValue2 === "starts with"));
                            depOpFieldObj.add(new Option("not starts with", "not starts with", null, currentValue2 === "not starts with"));
                            depOpFieldObj.add(new Option("ends with", "ends with", null, currentValue2 === "ends with"));
                            depOpFieldObj.add(new Option("not ends with", "not ends with", null, currentValue2 === "not ends with"));
                            depOpFieldObj.add(new Option("contains", "contains", null, currentValue2 === "contains"));
                            depOpFieldObj.add(new Option("equals", "=", null, currentValue2 === "="));

                            var inputElement = document.createElement('input')
                            inputElement.type = 'text';
                            inputElement.setAttribute('name', depFieldObj.getAttribute('name'));
                            inputElement.setAttribute('data-attrname', depFieldObj.getAttribute('data-attrname'));
                            if (currentValue1) {
                                inputElement.value = currentValue1;
                            }
                            var parent = depFieldObj.parentNode;
                            parent.replaceChild(inputElement, depFieldObj);
                        }
                        else if (attributedata.TypeID === 10 || attributedata.TypeID === 30 || attributedata.TypeID === 40 || attributedata.TypeID === 60 || attributedata.TypeID === 70 || attributedata.TypeID === 90 || attributedata.TypeID === 110 ||
                            attributedata.sBaseDataType === "int" || attributedata.sBaseDataType === "decimal") {
                            while (depOpFieldObj.options.length > 0) {
                                depOpFieldObj.remove(0);
                            }
                            depOpFieldObj.add(new Option("Please select", "", currentValue2 ?? true));
                            depOpFieldObj.add(new Option("equals", "=", null, currentValue2 === '='));
                            depOpFieldObj.add(new Option("greater than", ">", null, currentValue2 === '>'));
                            depOpFieldObj.add(new Option("less than", "<", null, currentValue2 === '<'));
                            depOpFieldObj.add(new Option("greater than or equal to", ">=", null, currentValue2 === '>='));
                            depOpFieldObj.add(new Option("less than or equal to", "<=", null, currentValue2 === '<='));
                            depOpFieldObj.add(new Option("not equal to", "!=", null, currentValue2 === '!='));
                            depOpFieldObj.add(new Option("between", "between", null, currentValue2 === 'between'));
                            depOpFieldObj.add(new Option("range", "range", null, currentValue2 === 'range'));

                            var inputElement = document.createElement('input')
                            inputElement.type = 'text';
                            inputElement.setAttribute('name', depFieldObj.getAttribute('name'));
                            inputElement.setAttribute('data-attrname', depFieldObj.getAttribute('data-attrname'));
                            if (currentValue1) {
                                inputElement.value = currentValue1;
                            }
                            var parent = depFieldObj.parentNode;
                            parent.replaceChild(inputElement, depFieldObj);

                            if (currentValue2 === "range") {
                                depOpFieldObj.dispatchEvent(new Event("change"));
                            }

                        }
                        else {
                            while (depOpFieldObj.options.length > 0) {
                                depOpFieldObj.remove(0);
                            }
                            depOpFieldObj.add(new Option("Please select", "", true));
                            depOpFieldObj.add(new Option("equals", "="));
                            depOpFieldObj.add(new Option("greater than", ">"));
                            depOpFieldObj.add(new Option("less than", "<"));
                            depOpFieldObj.add(new Option("greater than or equal to", ">="));
                            depOpFieldObj.add(new Option("less than or equal to", "<="));
                            depOpFieldObj.add(new Option("not equal to", "!="));

                            var inputElement = document.createElement('input')
                            inputElement.type = 'text';
                            inputElement.setAttribute('name', depFieldObj.getAttribute('name'));
                            inputElement.setAttribute('data-attrname', depFieldObj.getAttribute('data-attrname'));
                            var parent = depFieldObj.parentNode;
                            parent.replaceChild(inputElement, depFieldObj);


                        }
                    }
                }
            },
            error: function (data) {
            }
        })

    }

    fncGetAttributeRangeValues(event, attributeFieldName, whereFieldName, sRowIdentifier, currentValue) {
        debugger
        console.log(event.target.value, attributeFieldName, whereFieldName, sRowIdentifier, currentValue, attrFieldValue);
        const shadowRoot = document.querySelector('rules-component').shadowRoot;
        var rowObj = shadowRoot.querySelector("[data-id='" + sRowIdentifier + "']");
        var attrFieldElem = rowObj.querySelector("[data-attrname='" + attributeFieldName + "']");
        var whereFieldElem = rowObj.querySelector("[data-attrname='" + whereFieldName + "']");
        var sBaseDataType = whereFieldElem.getAttribute('data-typeId');

        if (event.target.value === "range") {
            var attrFieldValue = attrFieldElem.value;
            $.ajax({
                type: 'POST',
                url: LoadRangeWhereValuesURL,
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({ sAttributeIDXIGUID: attrFieldValue, sBaseDataType: sBaseDataType }),
                cache: false,
                async: true,
                dataType: 'json',
                success: (data) => {
                    debugger
                    if (data && data.length > 0) {
                        const selectTemplate = document.getElementById('select-template').content.cloneNode(true);
                        var selectObj = selectTemplate.getElementById("select-control");
                        selectObj.setAttribute('name', whereFieldElem.getAttribute('name'));
                        selectObj.setAttribute('data-attrname', whereFieldElem.getAttribute('data-attrname'));
                        selectObj.setAttribute('data-typeId', whereFieldElem.getAttribute('data-typeId'));
                        if (!currentValue) {
                            selectObj.add(new Option("Please select", "", null, true));
                        }
                        else {
                            selectObj.add(new Option("Please select", ""));
                        }

                        data.forEach(x => {
                            selectObj.add(new Option(x.sName, x.XIGUID, null, x.XIGUID === currentValue));
                        })
                        var parent = whereFieldElem.parentNode;
                        parent.replaceChild(selectObj, whereFieldElem);
                    }

                },
                error: (reason) => {
                    console.log(reason);
                }
            });
        }
        else {
            var inputElement = document.createElement('input');
            var typeId = whereFieldElem.getAttribute('data-typeId');
            if (typeId === '110') {
                inputElement.type = 'date';
                inputElement.setAttribute('data-typeId', typeId);
            }
            else if (typeId === '150' || typeId === 'datetime') {
                inputElement.type = 'datetime-local';
                inputElement.setAttribute('data-typeId', typeId);
            }
            else if (typeId === 'options') {

            }
            else {
                inputElement.type = 'text';
            }
            if (typeId !== 'options') {
                inputElement.setAttribute('name', whereFieldElem.getAttribute('name'));
                inputElement.setAttribute('data-attrname', whereFieldElem.getAttribute('data-attrname'));
                var parent = whereFieldElem.parentNode;
                parent.replaceChild(inputElement, whereFieldElem);
            }
        }
    }


}
customElements.define('rules-component', RulesComponent);
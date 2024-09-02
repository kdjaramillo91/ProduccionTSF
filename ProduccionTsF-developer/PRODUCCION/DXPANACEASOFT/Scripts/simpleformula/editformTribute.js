
var dataHashTag = function () {
    var tribute = null;
    var dataSet = Array();

    const clearDataSet = function () {
        dataSet = Array();
    };

    const GetDataSet = function () {
        //console.log(dataSet);
        return this.dataSet;
        
    };

    const tributeReplaced = function (e) {
        dataSet.push({
            "key": e.detail.item.original.value,
            "value": {
                "key": e.detail.item.original.keyInterna,
                "value": e.detail.item.original.value,
                "dataSource": e.detail.item.original.dataSource
            }
        });
    };

    const setDataHashTag = function () {

        tribute = new Tribute({
            trigger: "#",
            values: [
                {
                    key: "Jordan Humphreys",
                    value: "Jordan Humphreys",
                    email: "getstarted@zurb.com"
                },
                {
                    key: "Sir Walter Riley",
                    value: "Sir Walter Riley",
                    email: "getstarted+riley@zurb.com"
                },
                {
                    key: "Joachim",
                    value: "Joachim",
                    email: "getstarted+joachim@zurb.com"
                }
            ],
            selectTemplate: function (item) {
                if (typeof item === "undefined") return null;
                if (this.range.isContentEditable(this.current.element)) {
                    return (
                        `<span contenteditable="false"><a href="http://zurb.com" target="_blank" title="
						${item.original.email}">${item.original.value}</a></span>`
                    );
                }

                return (`#` + item.original.value);
            },
            requireLeadingSpace: false
        });
        if (!(document.getElementById("formulaTxt") == null || document.getElementById("formulaTxt") == undefined))
            tribute.attach(document.getElementById("formulaTxt"));
    };

    const setDataHashTagWithDataSources = function (dataSources, id_datasource) {

        let val = [];
        if (!(document.getElementById("formulaTxt") == null || document.getElementById("formulaTxt") == undefined)) {
            if (tribute != null)
                tribute.detach(document.getElementById("formulaTxt"));
            document.getElementById("formulaTxt").removeEventListener("tribute-replaced", tributeReplaced);
        }

        dataSources.forEach(function (item) {
            //console.log(item);
            val.push({
                key: item.name,
                value: item.name,
                keyInterna: item.id,
                dataSource: id_datasource
            });
        });

        tribute = new Tribute({
            trigger: "#",
            // menuContainer: document.getElementById('content'),
            values: val,
            selectTemplate: function (item) {
                if (typeof item === "undefined") return null;
                if (this.range.isContentEditable(this.current.element)) {
                    return (
                        `<span contenteditable="false"><a href="http://zurb.com" target="_blank">${item.original.value}</a></span>`
                    );
                }

                return (`{{${item.original.value}}}`);
            },
            requireLeadingSpace: false
        });
        if (!(document.getElementById("formulaTxt") == null || document.getElementById("formulaTxt") == undefined)) {
            tribute.attach(document.getElementById("formulaTxt"));
            document.getElementById("formulaTxt").addEventListener("tribute-replaced", tributeReplaced);
        }

    };

    return {
        onSetTributeNull: function () {
            if (!(document.getElementById("formulaTxt") == null || document.getElementById("formulaTxt") == undefined))
                tribute.detach(document.getElementById("formulaTxt"));
        },
        onSetDataHashTag: function () {
            setDataHashTag();
        },
        onSetDataHashTagWithDataSources: function (dataSources, id_datasource) {
            setDataHashTagWithDataSources(dataSources, id_datasource);
        },
        onGetDataSet: function () {
            return GetDataSet();
        },
        onClearDataSet: function () {
            clearDataSet();
        }
    };
};

var DataHashTag = new dataHashTag();

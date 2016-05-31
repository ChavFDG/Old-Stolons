
ProductTypesModel = Backbone.Collection.extend({
    url: "/api/ProductTypes",

    initialize: function() {
	this.fetch();
    }
});

//Juste pour la gestion de quelques evenements sur la vue globale
ManageProductView = Backbone.View.extend(
    {

	el: "body",

	events: {
	    "change #SellType": "sellTypeChanged"
	},

	initialize: function() {
	    this.sellTypeChanged();
	},

	sellTypeChanged: function(event) {
	    var sellType = $("#SellType").val();

	    if (sellType == 1) {
		//Vente à la pièce, on desactive tout ce qui concerne le poids
		$("#productWeightUnit").addClass("hidden");
		$("#productQtyStep").addClass("hidden");
		$("#productAvgWeight").addClass("hidden");
	    } else {
		$("#productWeightUnit").removeClass("hidden");
		$("#productQtyStep").removeClass("hidden");
		$("#productAvgWeight").removeClass("hidden");
	    }
	}
    }
);

ProductTypesView = Backbone.View.extend({

    el: "#famillySelect",

    template: _.template($("#familiesTemplate").html()),

    initialize: function(args) {
	this.model = args.model;
	this.listenTo(this.model, 'sync change', this.render);
	this.selectedFamily = "Tous";
    },

    onOptionSelected: function(selectedData) {
	this.selectedFamily = selectedData.params.data.id || "Tous";
    },

    selectElemTemplate: function(elem) {
	if (!elem.id) {
	    return elem.text;
	}
	var dataImage = $(elem.element).data("image");
	if (!dataImage) {
	    return elem.text;
	} else {
	    return $('<span class="select-option"><img src="/' + dataImage +'" />' + $(elem.element).text() + '</span>');
	}
    },

    render: function() {
	var currentFamilly = $("#productFamilly").text() || "Tous";
	currentFamilly = currentFamilly.trim();
	this.$el.html(this.template({ currentFamilly: currentFamilly, productTypes: this.model.toJSON() }));
	this.$('#familiesDropDown').select2({
	    minimumResultsForSearch: Infinity,
	    templateResult: this.selectElemTemplate,
	    templateSelection: this.selectElemTemplate
	});
	this.$('#familiesDropDown').on("select2:select", _.bind(this.onOptionSelected, this));
    }
});

$(function() {

    var productTypesModel = new ProductTypesModel();

    var view = new ProductTypesView({model: productTypesModel});

    var manageProductView = new ManageProductView({});

});



var CurrentModeModel = Backbone.Model.extend(
    {
	default: {mode: 0},

	url: "/api/currentMode",

	initialize: function() {
	    this.fetch();
	},

	parse: function(data) {
	    return {mode: data};
	}
    }
);

window.CurrentModeModel = new CurrentModeModel();

var ProductsCollection = Backbone.Collection.extend(
    {
	defaults: [],

	model: ProductModel,

	url: "/api/producerProducts",

	initialize: function() {
	    this.fetch();
	}
    }
);

window.ProductsModel = new ProductsCollection();

var StockMgtViewModal = Backbone.View.extend({

    el: "#stockMgt",

    events: {
	"change #WeekStock": "validateWeekStock",
	"change #RemainingStock": "validateRemainingStock",
	"click #saveStocks" : "saveStocks",
	"click #cancelEditStocks": "closeModal"
    },

    template: _.template($("#stockMgtTemplate").html()),

    initialize: function() {
	this.validation= {};
    },

    open: function(productId) {
	this.currentProduct = ProductsModel.get(productId);
	console.log(this.currentProduct);
	this.renderModal();
	this.validateWeekStock();
	this.validateRemainingStock();
    },

    isInt: function(n) {
	return Number(n) === n && n % 1 === 0;
    },

    validateWeekStock: function() {
	var weekStock = parseFloat($("#WeekStock").val());
	this.currentProduct.set({WeekStock: weekStock});

	if (this.currentProduct.get("Type") != 1) {
	    console.log("qtyStp = " + this.currentProduct.get("QuantityStep"));
	    if ((weekStock * 1000) % this.currentProduct.get("QuantityStep") != 0) {
		this.validation.weekStockError = "Le stock doit être divisible par le pallier de vente (" + this.currentProduct.get("QuantityStepString") + ").";
		this.render();
		return;
	    }
	}
	this.validation.weekStockError = "";
	this.render();
    },

    validateRemainingStock: function() {
	var remainingStock = parseFloat($("#RemainingStock").val());
	this.currentProduct.set({RemainingStock: remainingStock});

	if (this.currentProduct.get("Type") != 1) {
	    console.log("qtyStp = " + this.currentProduct.get("QuantityStep"));
	    if ((remainingStock * 1000) % this.currentProduct.get("QuantityStep") != 0) {
		this.validation.remainingStockError = "Le stock doit être divisible par le pallier de vente.";
 		this.render();
		return;
	    }
	}
	this.validation.remainingStockError = "";
	this.render();
    },

    saveStocks: function() {
	var self = this;
	$.ajax({
	    url: "/ProductsManagement/ChangeCurrentStock",
	    type: 'POST',
	    data: {
		id: self.currentProduct.get("Id"),
		newStock: self.currentProduct.get("RemainingStock"),
	    }
	}).always(function() {
	    $.ajax({
		url: "/ProductsManagement/ChangeStock",
		type: 'POST',
		data: {
		    id: self.currentProduct.get("Id"),
		    newStock: self.currentProduct.get("WeekStock"),
		}
	    }).always(function() {
		location.reload();
	    });
	});
	return false;
    },

    closeModal: function() {
	this.$el.modal('hide');
    },

    onClose: function() {
	this.currentProduct = null;
	this.$el.off('hide.bs.modal');
	this.$el.empty();
    },

    render: function() {
	this.$el.html(this.template(
	    {
		currentMode: window.CurrentModeModel.toJSON(),
		productModel: this.currentProduct,
		product: this.currentProduct.toJSON(),
		validation: this.validation
	    }
	));
    },

    renderModal: function() {
	this.render();
	this.$el.modal({keyboard: true, show: true});
	this.$el.on('hide.bs.modal', _.bind(this.onClose, this));
    }
});

window.StockMgtViewModal = new StockMgtViewModal({model: window.ProductsModel});

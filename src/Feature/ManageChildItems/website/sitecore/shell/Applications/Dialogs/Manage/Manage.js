Event.observe(document, "dom:loaded", function() {  
  scAttachEvents(); 
  var sortList = $("sort-list");
  if (sortList) {
    Sortable.create("sort-list", {scroll: "MainContainer", elements: $$(".sort-item.editable"),  format: /^([0-9a-zA-Z]{32})$/, onUpdate: function() {                   
          scPersistSortOrder();
        }
    });  
  }
});

function scAttachEvents() {
  $$(".sort-item.editable").each(function(item) {
    item.on("click", onItemClick);
    if (scForm.browser.isIE) {
      item.on("mouseenter", function() { this.addClassName("hover");});
      item.on("mouseleave", function() { this.removeClassName("hover");});
     }   
  });

  Event.observe(document.body, "click", function() {
    scClearSelectedItem();
    scUpdateMoveButtonsState();
  });
}

function onItemClick(e) {
  e.stop();
  scClearSelectedItem();
  this.addClassName("selected");
  scUpdateMoveButtonsState();
}

function scMoveUp() {
  var selectedItem = scGetSelectedItem();
  if (!selectedItem) {
    return false;
  }
  
  var previous = scGetPreviousItem(selectedItem);
  if (!previous) {
    return false;
  }
  
  previous.insert({before: selectedItem});
  scPersistSortOrder();
  scUpdateMoveButtonsState();
  return false;    
}

function scClearSelectedItem() {
  var selectedItem = scGetSelectedItem();
  if (selectedItem) {
    selectedItem.removeClassName("selected");
  }
}

function scMoveDown() {
  var selectedItem = scGetSelectedItem();
  if (!selectedItem) {
    return false;
  }
  
  var next = scGetNextItem(selectedItem);
  if (!next) {
    return false;
  }
  
  next.insert({after: selectedItem}); 
  scUpdateMoveButtonsState();
  scPersistSortOrder();
  return false;
}

function scGetSelectedItem() {
  return $$(".sort-item.selected")[0];
}

function scGetPreviousItem(item) {
  return item.previous(".sort-item");
}

function scGetNextItem(item) {
  return item.next(".sort-item");
}

function scUpdateMoveButtonsState() {
  var moveUp = $("MoveUp");
  var moveDown = $("MoveDown");
  var deleteBtn = $("Delete");
  var editBtn = $("Edit");
  var selectedItem = scGetSelectedItem();
  if (!selectedItem) {
    moveUp.disable();
    moveDown.disable();
	editBtn.disable();    
    return;
  }
  
  deleteBtn.enable();
  editBtn.enable();
  var next = scGetNextItem(selectedItem);
  if (next) {
    moveDown.enable();
  }
  else {
    moveDown.disable();
  }

  var previous = scGetPreviousItem(selectedItem);
  if (previous) {
    moveUp.enable();
  }
  else {
    moveUp.disable();
  }
}

function scPersistSortOrder() {
  var sortOrder = $("sortorder");
  if (!sortOrder) {
    sortOrder = new Element("input", {type: "hidden", id: "sortOrder"});
    var form = document.forms[0];
    if (!form) {
      return;
    }

    form.appendChild(sortOrder);
  }

  var ids = $$(".sort-item").map(function(item) { return item.id; }) || [];
  var serialized = ids.join("|") || "";
  sortOrder.value = serialized;
}

function scDelete() {    
  var deleteItem = $("deleteItem"); 
  if (!deleteItem) {        
    deleteItem = new Element("input", { type: "hidden", id: "deleteItem" });        
    var form = document.forms[0];        
    if (!form) {            
      return;        
    }
    form.appendChild(deleteItem);    
  } 
  scGetSelectedItem().addClassName("deleted");    
  var ids = $$(".deleted").map(function (item) { return item.id; }) || [];    
  var serialized = ids.join("|") || "";    
  deleteItem.value = serialized; 
  return false;
}

function scEdit() {
    var editItem = $("editItem");
    if (!editItem) {
        editItem = new Element("input", { type: "hidden", id: "editItem" });
        var form = document.forms[0];
        if (!form) {
            return;
        }
        form.appendChild(editItem);
    } 
    editItem.value = scGetSelectedItem().id;
    return scForm.postEvent(this, event, 'OnEdit');
}



/*
 *global variable, and helper methods, used to count how many product selections have changes,
* used later on during the installation for progress reporting
 */

var changedSelections = 0;

function IncrementSelectionsCount()
{
    changedSelections++;
    external.MsiSetProperty("CHANGED_SELECTIONS", changedSelections.toString());
}


function DecrementSelectionsCount()
{
    changedSelections--;
    external.MsiSetProperty("CHANGED_SELECTIONS", changedSelections.toString());   
}


/*
 * Method used to set installer properties based user selection
 * from the HTML view.
 *
 * example of call:  SelectProduct("#controlId", "CONTROL_PROP");
 */

function SelectProduct(aProdId, aProdProperty)
{
    $(aProdId).click(function() {

    if($(this).is(":checked"))
    {
        external.MsiSetProperty(aProdProperty, 'set');
        IncrementSelectionsCount();
    }
    else
    {
            // delete property
        external.MsiSetProperty(aProdProperty, '[~]');
        DecrementSelectionsCount();
    }

  });
}

/*
 * Method used to tick a checknox based on the value of a property.
 *
 * example of call:  TickCheckbox("#checkboId", "CHECKBOX_PROP");
 */

function TickCheckbox(aProdId, aProdProperty)
{
    if (external.MsiGetProperty(aProdProperty))
    {
        //alert( external.MsiGetProperty(aProdProperty) );
        $(aProdId).prop('checked', true);
        external.MsiSetProperty(aProdId.substr(1) + '_PROP', 'set');
        IncrementSelectionsCount();
    }
}

/*
 * Method used to set initial products names, identical with what is on Chocolatey,
 * to be used by the "InstallSelectedApplications" custom action.
 */


function InitProdandRepoNamesProps()
{
    //prod names
    external.MsiSetProperty("NSB_PROD_NAME", 'NServiceBus');
    external.MsiSetProperty("SC_PROD_NAME", 'ServiceControl');
    external.MsiSetProperty("SI_PROD_NAME", 'ServiceInsight');    
    external.MsiSetProperty("SP_PROD_NAME", 'ServicePulse');
    external.MsiSetProperty("SM_PROD_NAME", 'ServiceMatrix');


    //repo names
    external.MsiSetProperty("NSB_REPO_NAME", 'Particular/NServiceBus');
    external.MsiSetProperty("SC_REPO_NAME", 'Particular/ServiceControl');
    external.MsiSetProperty("SI_REPO_NAME", 'Particular/ServiceInsight');
    external.MsiSetProperty("SP_REPO_NAME", 'Particular/ServicePulse');
    external.MsiSetProperty("SM_REPO_NAME", 'Particular/ServiceMatrix');   
}


/*
 * Method used to enabled/disable SI and SP checkboxes based on SC selection.
 * 
 */

function ToogleSIandSPCheckboxes() {
  if (this.checked) {
    $("input.ckb").removeAttr("disabled");    
  } else {
    $("input.ckb").attr("disabled", true);    
  }
}

// Because IE does not work correctly with Toggle() from jQuery
// The below should work, but still is not. In Chrome it works

function Toggle(aClassname){
    var elem = $(aClassname)[0];
    if(elem.style.display == 'none')
         $(aClassname).show();
    else
    {
         $(aClassname).hide();               
    }
}
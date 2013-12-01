/*
 *global variable, and helper methods, used to count how many product selections have changes,
* used later on during the installation for progress reporting
 */

var changedSelections = 0;

function IncrementSelectionsCount(aProdId)
{
    if ( external.MsiGetProperty(aProdId.substr(1) + '_SEARCH') )
        {            
            changedSelections--; // this is the case where the user unchecks and checks back an installed component 
        }
    else
        {
            changedSelections++;            
        }

    external.MsiSetProperty("CHANGED_SELECTIONS", changedSelections.toString());    
}


function DecrementIncrementSelectionsCount(aProdProperty)
{
    if (aProdProperty == "NSB_PROP" && StringIsEmpty(external.MsiGetProperty("NSB_SEARCH")))
        changedSelections--;
    else if (aProdProperty == "NSB_PROP" && external.MsiGetProperty("NSB_SEARCH"))
        changedSelections++;
        
    if (aProdProperty == "SC_PROP" && StringIsEmpty(external.MsiGetProperty("SC_SEARCH")))
        changedSelections--;
    else if (aProdProperty == "SC_PROP" && external.MsiGetProperty("SC_SEARCH"))
        changedSelections++;

   if (aProdProperty == "SI_PROP" && StringIsEmpty(external.MsiGetProperty("SI_SEARCH")))
        changedSelections--;
    else if (aProdProperty == "SI_PROP" && external.MsiGetProperty("SI_SEARCH"))
        changedSelections++;        
      
   if (aProdProperty == "SP_PROP" && StringIsEmpty(external.MsiGetProperty("SP_SEARCH")))
        changedSelections--;
    else if (aProdProperty == "SP_PROP" && external.MsiGetProperty("SP_SEARCH"))
        changedSelections++;        
        
   if (aProdProperty == "SAMP_PROP" && StringIsEmpty(external.MsiGetProperty("SAMP_SEARCH")))
        changedSelections--;
    else if (aProdProperty == "SAMP_PROP" && external.MsiGetProperty("SAMP_SEARCH"))
        changedSelections++;        
        
   if (aProdProperty == "TOOL_PROP" && StringIsEmpty(external.MsiGetProperty("TOOL_SEARCH")))
        changedSelections--;
    else if (aProdProperty == "TOOL_PROP" && external.MsiGetProperty("TOOL_SEARCH"))
        changedSelections++;        
   
    external.MsiSetProperty("CHANGED_SELECTIONS", changedSelections.toString());   
}

function StringIsEmpty(aString) {
    return (!aString || 0 === aString.length);
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
        IncrementSelectionsCount(aProdId);
    }
    else
    {
            // delete property
        external.MsiSetProperty(aProdProperty, '[~]');
        DecrementIncrementSelectionsCount(aProdProperty);
    }

  });
}


/*
 * Method used to use uncheck and delete property for a control.
 * Usefull when toogling controls based on another's control state.
 * 
 * example of call:  DeSelectProduct("#controlId", "CONTROL_PROP");
 */
function DeSelectProduct(aProdId, aProdProperty)
{
   if(external.MsiGetProperty(aProdProperty))
    {
        external.MsiSetProperty(aProdProperty, '[~]');
        $(aProdId).removeAttr("checked");  
        DecrementIncrementSelectionsCount(aProdProperty);
    }
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
    DeSelectProduct("#SI", "SI_PROP");
    DeSelectProduct("#SP", "SP_PROP");    
  }
}


/*
 * Method used to enabled/disable SI and SP checkboxes based on SC search, just when the installer is launched.
 */

function LoadToogleSIandSPCheckboxes() {
  if (external.MsiGetProperty("SC_SEARCH")) {
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
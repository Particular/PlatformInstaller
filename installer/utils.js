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
    }
    else
    {
            // delete property
        external.MsiSetProperty(aProdProperty, '[~]');
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
    }
}
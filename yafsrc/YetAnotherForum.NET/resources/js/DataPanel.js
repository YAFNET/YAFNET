function DataPanel_ExpandCollapse(hd, cht, cha, st, tc, te)
{ 
	if(document.getElementById(hd).style.display == '')
	{ 
		document.getElementById(hd).style.display = 'none';
		if(cht != '')
		{
			document.getElementById(cht).title = te;
		}
		if(document.getElementById(cha) != null)
		{
			document.getElementById(cha).innerHTML = te;
			document.getElementById(cha).title = te;
		}
		document.getElementById(st).value = 'true';
	}
	else
	{
		document.getElementById(hd).style.display = '';
		if(cht != '')
		{
			document.getElementById(cht).title = tc;
		}
		if(document.getElementById(cha) != null)
		{
			document.getElementById(cha).innerHTML = tc;
			document.getElementById(cha).title = tc;
		}
		document.getElementById(st).value = 'false';
	}
}
function DataPanel_ExpandCollapseImage(hd, cht, cha, st, ex, cl, tc, te)
{
	var elImg = (document.getElementById(cha)).getElementsByTagName("img");
	if(document.getElementById(hd).style.display == '')
	{ 
		document.getElementById(hd).style.display = 'none';
		if(cht != '')
		{
			document.getElementById(cht).title = te;
		}
		elImg[0].src = ex;
		elImg[0].alt = te;
		elImg[0].title = te; // logan fix
		document.getElementById(st).value = 'true';
	}
	else
	{ 
		document.getElementById(hd).style.display = '';
		if(cht != '')
		{
			document.getElementById(cht).title = tc;
		}
		elImg[0].src = cl;
		elImg[0].alt = tc;
		elImg[0].title = te; // logan fix
		document.getElementById(st).value = 'false';
	}
}
using UnityEngine;
using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

//using System.Linq.Expressions;
public class InspectorPart : Part{

	/*
This module will create a new GUI window and button, on button press the part will explode, close the GUI, and be removed.
*/

		protected Rect windowPos;
		private Vector2 _scrollPosition;
		private  float _width = 300f;
		private float _height = 300f;
	private FieldInfo[] reflect <T> (T ThisOb)
	{
		Type type = typeof(T);

		FieldInfo[] fields = type.GetFields (BindingFlags.Instance | BindingFlags.Public);
		List<FieldInfo> list = new List<FieldInfo> (fields);
		List<FieldInfo> listDelete = new List<FieldInfo> ();
		foreach (FieldInfo field in list) {
			if (field.Name == null)
				listDelete.Add (field);
		}
		foreach (FieldInfo field in listDelete) {
			list.Remove (field);
		}
		fields = list.ToArray();
		return fields;

			//field.SetValue(Root.vessel, Root.transform);
			//debugprint(field.GetType().ToString()); 
			//debugprint(field.ReflectedType.GetType().ToString());

	}

//	static string GetVariableName<T>(Expression<Func<T>> expr)
//{
//    var body = (MemberExpression)expr.Body;
//
//    return body.Member.Name;
//}
private List<string> Format (FieldInfo[] fields,object obj)
	{
		string varname = "missing";
		string value = "missing";
		string type = "missing";
		string line= "missing";
		List<string> outList = new List<string>();

		foreach (FieldInfo f in fields) {
			try {
				varname = f.Name;
				type = f.FieldType.ToString();
				if (!f.IsStatic) value = f.GetValue (obj).ToString (); 
				if (f.IsStatic ) value = f.GetValue (null).ToString();
			} 
			catch{}
		}



	}
	private string FormatValue (object o)
	{
		if (o == null)
			return "null";
		if (o is double | o is float) {
			if(o>1000000)
			return o.ToString("E2");		
			return o.ToString ("f2");
		}
		return o.ToString();
		}
	}
 
		private void WindowGUI (int windowID)
	{
		GUIStyle mySty = new GUIStyle (GUI.skin.button); 
		mySty.normal.textColor = mySty.focused.textColor = Color.white;
		mySty.hover.textColor = mySty.active.textColor = Color.yellow;
		mySty.onNormal.textColor = mySty.onFocused.textColor = mySty.onHover.textColor = mySty.onActive.textColor = Color.green;
		mySty.padding = new RectOffset (8, 8, 8, 8);
 
		GUILayout.BeginVertical ();
		_scrollPosition = GUILayout.BeginScrollView (_scrollPosition, GUILayout.Width (300), GUILayout.Height (300));
		GUILayout.Box ("label");
//		List<string> items = ObjectDumper.Dump (vessel);
//		foreach (string line in items) {
//			GUILayout.Label (line);
//		}

		FieldInfo[] fields = reflect<Vessel> (vessel);
		foreach (var f in fields) {
			string varname = "f";
			string value ="xxx";
			string type = "";
			//varname = GetVariableName(() => f);
			type = " : "+f.FieldType.ToString()+" ";
			//value =f.GetValue(null).ToString();
			print (f.Name);
			try{
				if(!f.IsStatic){
					value = f.GetValue(vessel).ToString (); 
					print (value);
				}
			}
			catch(Exception e)
			{
				value =e.ToString();
				print ( value);

			}

			//print (f.GetValue(null).ToString());
			//print (type+" "+value);
			GUILayout.Label(f.Name + type +value);

		}
		GUILayout.EndScrollView();
		GUILayout.EndVertical();
 
                //DragWindow makes the window draggable. The Rect specifies which part of the window it can by dragged by, and is 
                //clipped to the actual boundary of the window. You can also pass no argument at all and then the window can by
                //dragged by any part of it. Make sure the DragWindow command is AFTER all your other GUI input stuff, or else
                //it may "cover up" your controls and make them stop responding to the mouse.
                GUI.DragWindow(new Rect(0, 0, 10000, 20));
 
		}
		private void drawGUI()
		{
            GUI.skin = HighLogic.Skin;
            windowPos = GUILayout.Window(1, windowPos, WindowGUI, "Self Destruct", GUILayout.MinWidth(100));	 
		}
		protected override void onFlightStart()  //Called when vessel is placed on the launchpad
		{
		    RenderingManager.AddToPostDrawQueue(3, new Callback(drawGUI));//start the GUI
		}
		protected override void onPartStart()
		{
       		if ((windowPos.x == 0) && (windowPos.y == 0))//windowPos is used to position the GUI window, lets set it in the center of the screen
			{
          	  windowPos = new Rect(Screen.width / 2, Screen.height / 2, 10, 10);
   			}
		}
	 	protected override void onPartDestroy() 
		{
			RenderingManager.RemoveFromPostDrawQueue(3, new Callback(drawGUI)); //close the GUI
   		}
 

}

public class ObjectDumper
{
	private int _levelLimit =2;
    private int _level;
    private readonly int _indentSize;        
    private readonly StringBuilder _stringBuilder;
	private readonly List<String> _stringList;

//	public static List<string> DumpList (object element)
//	{
//		Dump(element);
//		return _stringList;
//
//	}
// Example code pulled from forum
    private ObjectDumper(int indentSize)
    {
        _indentSize = indentSize;
        _stringBuilder = new StringBuilder();
		_stringList = new List<string>();
    }

    public static List<string> Dump(object element)
    {
        return Dump(element, 2);
    }

    public static List<string> Dump(object element, int indentSize)
    {
        var instance = new ObjectDumper(indentSize);
        return instance.DumpElement(element);
    }

    private List<string> DumpElement(object element)
    {
        if (element == null || element is ValueType || element is string)
        {
            Write(FormatValue(element));
        }
        else
        {
            var objectType = element.GetType();
            if (!typeof (IEnumerable).IsAssignableFrom(objectType))
            {
                Write("{{{0}}}", objectType.FullName);
                _level++;
            }

            var enumerableElement = element as IEnumerable;
            if (enumerableElement != null)
            {
                foreach (object item in enumerableElement)
                {
                    if (item is IEnumerable && !(item is string))
                    {
						if (_level < _levelLimit){
	                        _level++;
	                        DumpElement(item);
	                        _level--;
						}
                    }
                    else
                    {
                        DumpElement(item);
                    }
                }
            }
            else
            {
                MemberInfo[] members = element.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);
                foreach (var memberInfo in members)
                {
                    var fieldInfo = memberInfo as FieldInfo;
                    var propertyInfo = memberInfo as PropertyInfo;

                    if (fieldInfo == null && propertyInfo == null)
                        continue;

                    var type = fieldInfo != null ? fieldInfo.FieldType : propertyInfo.PropertyType;
                    object value = fieldInfo != null
                                       ? fieldInfo.GetValue(element)
                                       : propertyInfo.GetValue(element, null);

                    if (type.IsValueType || type == typeof(string))
                    {
                        Write("{0}: {1}", memberInfo.Name, FormatValue(value));
                    }
                    else
                    {
                        Write("{0}: {1}", memberInfo.Name, typeof(IEnumerable).IsAssignableFrom(type) ? "..." : "{ }");
						if(_level<_levelLimit){
	                        _level++;
	                        DumpElement(value);
	                        _level--;
						}
                    }
                }                    
            }

            if (!typeof(IEnumerable).IsAssignableFrom(objectType))
            {
                _level--;
            }
        }

       // return _stringBuilder.ToString();
		return _stringList;
    }

    private void Write(string value, params object[] args)
    {
        var space = new string(' ', _level * _indentSize);

        if (args != null)
            value = string.Format(value, args);

        _stringBuilder.AppendLine(space + value);
		_stringList.Add (space + value);
    }

    private string FormatValue(object o)
    {
        if (o == null)
            return ("null");

        if (o is DateTime)
            return (((DateTime)o).ToShortDateString());

        if (o is string)
            return string.Format("\"{0}\"", o);

        if (o is ValueType)
            return (o.ToString());

        if (o is IEnumerable)
            return ("...");

        return ("{ }");
    }
}

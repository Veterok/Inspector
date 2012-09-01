using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

	public class ObjectList <X> {
	object rootObject;
		//	FieldInfo[] FieldArray;
			 private int _level =0;
			 public List<String> Entries;
			 List<FieldInfo> Fields;
			 List<object > Objects;
	public string Name{
		get {return rootObject.ToString();}
	}
	public int level {
		get{ return _level;}
	}
	public ObjectList(object root){
				rootObject =root;
				Entries = new List<string>();
				Fields = new List<FieldInfo>();
				Objects = new List<object>();

			}
		public ObjectList(object root,int level){
			_level = level;
			rootObject =root;
			Entries = new List<string>();
			Fields = new List<FieldInfo>();
			Objects = new List<object>();

		}
	public void Add (FieldInfo Field, object NewObj)
		{
			Entries.Add (Format (Field, rootObject));
			Fields.Add (Field);
		}
		public void Clear ()
		{
			Fields.Clear ();
			Entries.Clear ();
		}
		public int Count
		{
			get{ return Fields.Count;}
		}
		public void Remove (FieldInfo Field)
		{
			Entries.RemoveAt(Fields.BinarySearch (Field));
			Fields.Remove(Field);
		}
		public void Remove (string Entry)
		{
			Fields.RemoveAt(Entries.BinarySearch(Entry));
			Entries.Remove(Entry);
		}

		protected virtual string Format (FieldInfo f,object obj)
	{
			
			string name = "";
			string type = "";
			string _type = "";
			string value = "";
			object _value = null;
			try {name = f.Name;}
		catch(Exception e){ name = e.ToString();}
			try{
			_type = f.FieldType.ToString();
			//print(_type.ToString());
			//print(_type.GetType().ToString());
			int lastdot = _type.LastIndexOf('.');
				if (lastdot !=-1) type = _type.Substring(lastdot+1);
		}
		catch(Exception e){ type = e.ToString();}
		try{
				if (!f.IsStatic) _value = f.GetValue(obj); 
				if (f.IsStatic ) _value = f.GetValue(null);
			value = FormatValue(_value,_type);
		}
		//catch(InvalidCastException e){value = value.ToString();}
		catch(Exception e){ value = e.ToString();}
			return(type+" "+name+" : "+value).Trim();
		//return type +":"+ _type.ToString()+":"+_type.GetType().ToString()+":"+typeof(double).ToString(); //_type
	}
	protected virtual string FormatValue(object o,string _type)
	{
		if (o == null)
			return "null";

		if ( _type == typeof(float).ToString() | _type == typeof(double).ToString()) {
			return reFormat((float)Convert.ChangeType(o,typeof(float)));
		}
		if ( _type == typeof(double).ToString()) {
			return reFormat((float)Convert.ChangeType(o,typeof(float)));
		}
		if (_type == typeof(Vector3).ToString()) {
			return reFormat((Vector3)Convert.ChangeType(o,typeof(Vector3)));

		}
		if (_type == typeof(Vector3d).ToString() ) {
			return reFormat((Vector3d)Convert.ChangeType(o,typeof(Vector3d)));
			

		}


		return o.ToString();
		}


	public void Update ()
	{
		this.Clear ();
		//Vessel v = new Vessel ();
		Fields= reflect <X> ((X)rootObject);
		foreach (var f in Fields) {
			Entries.Add(Format(f,rootObject));
		}

//		FieldInfo[] fields = reflect<Vessel> (vessel);
//		foreach (var f in fields) {
//			string varname = "f";
//			string value = "xxx";
//			string type = "";
//			//varname = GetVariableName(() => f);
//			type = " : " + f.FieldType.ToString () + " ";
//			//value =f.GetValue(null).ToString();
//			print (f.Name);
//			try {
//				if (!f.IsStatic) {
//					value = f.GetValue (vessel).ToString (); 
//					print (value);
//				}
//			} catch (Exception e) {
//				value = e.ToString ();
//				print (value);

		}

			//print (f.GetValue(null).ToString());
			//print (type+" "+value);

		
		private List<FieldInfo> reflect <T> (T ThisOb)
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
		//fields = list.ToArray();

		return list;

			//field.SetValue(Root.vessel, Root.transform);
			//debugprint(field.GetType().ToString()); 
			//debugprint(field.ReflectedType.GetType().ToString());

	}


	 public static string reFormat (float num)
	{
		if (num > 1000000) {
				return num.ToString ("E2");
			}
			return num.ToString ("f2");
}
	public static string reFormat (double num)
	{
		return reFormat ((float)num);
}
	public static string reFormat (Vector3 vector)
	{
		string x;
		string y;
		string z;
		x = "["+reFormat(vector.x)+"]";
		y = "["+reFormat(vector.y)+"]";
		z = "["+reFormat(vector.z)+"]";

		return x+y+z;

	}
	public static string reFormat (Vector3d vector)
	{
		string x;
		string y;
		string z;
		x = "["+reFormat(vector.x)+"]";
		y = "["+reFormat(vector.y)+"]";
		z = "["+reFormat(vector.z)+"]";

		return x+y+z;

	}
	void SetValue(PropertyInfo info, object instance, object value)
{
    var targetType = info.PropertyType.IsNullableType() ? Nullable.GetUnderlyingType(info.PropertyType) : info.PropertyType; 
    var convertedValue = Convert.ChangeType(value, targetType);

    info.SetValue(instance, convertedValue, null);
}


}
public static class TypeExtensions
{
  public static bool IsNullableType(this Type type)
  {
    return type.IsGenericType 
    && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
  }
}



using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

using CookComputing.XmlRpc;

using PropertyList = System.Collections.Generic.Dictionary<string, object>;

namespace TestPlant.EggDriver
{

	[XmlRpcMissingMapping(MappingAction.Ignore)]
	public class EggDriveResponse
	{
		public string Output;
		public double Duration;
		public object ReturnValue;
		public object Result;

		public PropertyList ReturnValueAsPropertyList ()
		{
			var h = ReturnValue as Hashtable;
			if (h == null)
				return null;

			return h
				.Cast<DictionaryEntry> ()
				.ToDictionary (kvp => (string)kvp.Key, kvp => (object)kvp.Value);
		}

		public PropertyList[] ReturnValueAsPropertyLists ()
		{
			var list = ReturnValue as object[];
			var hashtables = list == null ? null : list.Cast<Hashtable> ().ToArray ();

			if (hashtables == null)
				return null;

			var mutable = new List<PropertyList> ();
			foreach (Hashtable h in hashtables) {
				mutable.Add (h.Cast<DictionaryEntry> ()
					.ToDictionary (kvp => (string)kvp.Key, kvp => (object)kvp.Value));
			}

			return mutable.ToArray ();
		}

		public Point ReturnValueAsPoint ()
		{
			var point = ReturnValue as double[];
			int x = -1;
			int y = -1;

			if (point != null && point.Length >= 2) {
				x = (int)point[0];
				y = (int)point[1];
			}

			return new Point(x, y);
		}

		public Point[] ReturnValueAsPoints ()
		{
			var list = ReturnValue as object[];
			if (list != null)
			{
				var points = new List<Point> ();
				foreach (object o in list)
				{
					var point = o as double[];
					if (point != null && point.Length >= 2)
						points.Add(new Point((int)point[0], (int)point[1]));
				}

				return points.ToArray ();
			}

			return null;
		}

		public Size ReturnValueAsSize ()
		{
			Point p = ReturnValueAsPoint ();
			return new Size (p.X, p.Y);
		}

		public Size[] ReturnValueAsSizes ()
		{
			var points = ReturnValueAsPoints ();
			var sizes = new List<Size> ();

			foreach (Point p in points)
				sizes.Add (new Size (p.X, p.Y));

			return sizes.ToArray ();
		}
	}

	public class EggDriveXmlRpcConnector
	{
		public bool OverridePreviousSession = true;
		public bool DebugEnabled = false;

		public string Url
		{
			get {
				return url;
			}
			set {
				url = value;
				proxy.Url = url;
			}
		}

		private string url = "http://localhost:5400";
		private readonly IEggDrive proxy = XmlRpcProxyGen.Create<IEggDrive>();

		protected interface IEggDrive : IXmlRpcProxy
		{
			[XmlRpcMethod("StartSession")]
			void StartSession();

			[XmlRpcMethod("StartSession")]
			void StartSession(string RemoteScriptPath);

			[XmlRpcMethod("EndSession")]
			void EndSession();

			[XmlRpcMethod("Execute")]
			EggDriveResponse Execute(string Command);
		}

		public EggDriveXmlRpcConnector () 
		{
			// call getter/setter
			this.Url = this.Url;
		}

		public EggDriveXmlRpcConnector (string url)
		{
			this.Url = url;
		}

		public void StartSession ()
		{
			StartSession (null);
		}

		public void StartSession (string remoteSuitePath)
		{
			try {
				if (remoteSuitePath != null) {
					Debug ("Starting EggDrive session with suite: " + remoteSuitePath);
					proxy.StartSession (remoteSuitePath);
				}
				else {
					Debug ("Starting EggDrive session without suite");
					proxy.StartSession ();
				}
			}
			catch (XmlRpcFaultException faultException)
			{
				if (OverridePreviousSession && (EggDriveFaultCode)faultException.FaultCode == EggDriveFaultCode.SessionBusy) {
					try {

						Debug ("Ending existing session");
						proxy.EndSession ();

						if (remoteSuitePath != null) {
							Debug ("Starting new session with suite: " + remoteSuitePath);
							proxy.StartSession (remoteSuitePath);
						}
						else {
							Debug ("Starting new session without suite");
							proxy.StartSession ();
						}
					} 
					catch (XmlRpcFaultException secondFaultException)
					{
						throw new EggDriveException (
							(EggDriveFaultCode)secondFaultException.FaultCode, 
							secondFaultException.FaultString, 
							secondFaultException);
					}
				} 
				else throw new EggDriveException (
					(EggDriveFaultCode)faultException.FaultCode, 
					faultException.FaultString, 
					faultException);
			}
		}

		public void StartSession (string remoteScriptPath, string url) 
		{
			this.Url = url;
			StartSession (remoteScriptPath);
		}

		public void EndSession ()
		{
			try {
				Debug ("Ending session");
				proxy.EndSession ();
			} 
			catch (XmlRpcFaultException faultException)
			{
				throw new EggDriveException (
					(EggDriveFaultCode)faultException.FaultCode, 
					faultException.FaultString, 
					faultException);
			}
		}

		public EggDriveResponse Execute (string command)
		{
			Debug ("Executing: " + command);

			try {
				return proxy.Execute (command);
			}
			catch (XmlRpcFaultException faultException)
			{
				throw new EggDriveException (
					(EggDriveFaultCode)faultException.FaultCode, 
					faultException.FaultString, 
					faultException);
			}
		}

		private void Debug(string s) {
			if (DebugEnabled)
				Console.WriteLine (s);
		}

	}
}


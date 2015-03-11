using System;
using System.Drawing;
using System.Collections.Generic;

using PropertyList = System.Collections.Generic.Dictionary<string, object>;

namespace TestPlant.EggDriver
{
	public enum EggplantConnectionType { VNC, RDP }

	public class EggDriver : EggDriveXmlRpcConnector
	{

		public EggDriver(): base() {}
		public EggDriver(string eggDriveURL): base(eggDriveURL) {}

		public object EvaluateExpression (object expression)
		{
			var statement = (new SenseTalkStatementBuilder (expression, SenseTalkStatementType.Expression)).ToString ();
			var response = Execute (statement);
			return response.ReturnValue;
		}

#region Logging

		public void Log (string s)
		{
			Execute ((new SenseTalkStatementBuilder ("Log"))
				.AddQuotedParameter (s)
				.ToString());
		}

		public void LogExpression (object expression)
		{
			Execute ((new SenseTalkStatementBuilder ("Log"))
				.AddParameter (expression)
				.ToString());
		}

		public void LogError (string s)
		{
			Execute ((new SenseTalkStatementBuilder ("LogError"))
				.AddQuotedParameter (s)
				.ToString());
		}

		public void LogErrorExpression (object expression)
		{
			Execute ((new SenseTalkStatementBuilder ("LogError"))
				.AddParameter (expression)
				.ToString());
		}

#endregion

#region SUT connections

		/**
		 * Instruct EggDrive to connect to a SUT.
		 * 
		 * @param serverID    The hostname, IP address, or Connection List Display Name.
		 *                    This is the only required argument.
		 * @param portNum     The port number used by the server on the SUT. (Default value: 5900.)
		 * @param type        VNC or RDP. (Default value: VNC.)
		 * @param username    The Windows username when connecting via RDP.
		 * @param password    The password for the VNC server if you're making a VNC connection, 
		 *                    or the Windows user password if you're making an RDP connection. 
		 *                    (Note that RDP connections require a password.)
		 * @param sshHost     The hostname or IP address of a computer hosting an SSH connection.
		 * @param sshUser     The user account on the SSH host
		 * @param sshPassword The password to the user account on the SSH server.
		 * @param visible     Whether or not the Viewer window automatically opens upon connection.
		 * @param colorDepth  The color depth of the SUT in the Viewer window: 8, 16, 32. 
		 *                    (Default value: The native color depth of the SUT.)
		 */
		public void ConnectToSUT (
			string serverID,
			int? portNum = null,
			EggplantConnectionType? type = null,
			string username = null,
			string password = null,
			string sshHost = null,
			string sshUser = null,
			string sshPassword = null,
			bool? visible = null,
			int? colorDepth = null)
		{
			if (serverID == null)
				throw new ArgumentException ("Argument is required", "serverID");

			if (portNum.HasValue && portNum <= 0)
				portNum = null;

			Execute ((new SenseTalkStatementBuilder ("Connect"))
				.AddQuotedPropertyListParameter ("ServerID", serverID)
				.AddPropertyListParameter ("PortNum", portNum)
				.AddPropertyListParameter ("Type", type)
				.AddQuotedPropertyListParameter ("Username", username)
				.AddQuotedPropertyListParameter ("Password", password)
				.AddQuotedPropertyListParameter ("sshHost", sshHost)
				.AddQuotedPropertyListParameter ("sshUser", sshUser)
				.AddQuotedPropertyListParameter ("sshPassword", sshPassword)
				.AddPropertyListParameter ("Visible", visible)
				.AddPropertyListParameter ("ColorDepth", colorDepth)
				.ToString());
		}

		public PropertyList ConnectionInfo (string connectionName = null)
		{
			var statement = new SenseTalkStatementBuilder ("ConnectionInfo", SenseTalkStatementType.Function)
				.AddQuotedParameter (connectionName)
				.ToString ();
			return Execute (statement).ReturnValueAsPropertyList ();
		}

#endregion

#region Remote screen dimensions

		public Size RemoteScreenSize ()
		{
			return Execute ((new SenseTalkStatementBuilder ("RemoteScreenSize", SenseTalkStatementType.Function))
				.ToString()).ReturnValueAsSize ();
		}

		public Point RemoteScreenCenter ()
		{
			Size remoteSize = RemoteScreenSize ();
			return new Point (remoteSize.Width / 2, remoteSize.Height / 2);
		}

#endregion

#region Pointer events

		public void Click (Point point) {
			ExecuteGestureAtPoint ("Click", point);
		}

		public void Click (string imageName, double? searchTimeoutSeconds = null) {
			ExecuteGestureAtImage ("Click", imageName, searchTimeoutSeconds);
		}

		public void ClickText (string text, PropertyList options = null) {
			ExecuteGestureAtText ("Click", text, options);
		}

		public void DoubleClick (Point point) {
			ExecuteGestureAtPoint ("DoubleClick", point);
		}

		public void DoubleClick (string imageName, double? searchTimeoutSeconds = null) {
			ExecuteGestureAtImage ("DoubleClick", imageName, searchTimeoutSeconds);
		}

		public void DoubleClickText (string text, PropertyList options = null) {
			ExecuteGestureAtText ("DoubleClick", text, options);
		}

		public void RightClick (Point point) {
			ExecuteGestureAtPoint ("RightClick", point);
		}

		public void RightClick (string imageName, double? searchTimeoutSeconds = null) {
			ExecuteGestureAtImage ("RightClick", imageName, searchTimeoutSeconds);
		}

		public void RightClickText (string text, PropertyList options = null) {
			ExecuteGestureAtText ("RightClick", text, options);
		}

		public void MoveTo (Point point) {
			ExecuteGestureAtPoint ("MoveTo", point);
		}

		public void MoveTo (string imageName, double? searchTimeoutSeconds = null) {
			ExecuteGestureAtImage ("MoveTo", imageName, searchTimeoutSeconds);
		}

		public void MoveToText (string text, PropertyList options = null) {
			ExecuteGestureAtText ("MoveTo", text, options);
		}

#endregion

#region Gestures

		public void Tap (Point point) {
			ExecuteGestureAtPoint ("Tap", point);
		}

		public void Tap (string imageName, double? searchTimeoutSeconds = null) {
			ExecuteGestureAtImage ("Tap", imageName, searchTimeoutSeconds);
		}

		public void TapText (string text, PropertyList options = null) {
			ExecuteGestureAtText ("Tap", text, options);
		}

		public void SwipeLeft () {
			ExecuteGestureAtPoint ("SwipeLeft");
		}

		public void SwipeLeft (Point point) {
			ExecuteGestureAtPoint ("SwipeLeft", point);
		}

		public void SwipeLeft (string imageName) {
			ExecuteGestureAtImage ("SwipeLeft", imageName);
		}

		public void SwipeLeftText (string imageName, PropertyList options = null) {
			ExecuteGestureAtText ("SwipeLeft", imageName, options);
		}

		public void SwipeRight () {
			ExecuteGestureAtPoint ("SwipeRight");
		}

		public void SwipeRight (Point point) {
			ExecuteGestureAtPoint ("SwipeRight", point);
		}

		public void SwipeRight (string imageName) {
			ExecuteGestureAtImage ("SwipeRight", imageName);
		}

		public void SwipeRightText (string imageName, PropertyList options = null) {
			ExecuteGestureAtText ("SwipeRight", imageName, options);
		}

		public void SwipeDown () {
			ExecuteGestureAtPoint ("SwipeDown");
		}

		public void SwipeDown (Point point) {
			ExecuteGestureAtPoint ("SwipeDown", point);
		}

		public void SwipeDown (string imageName) {
			ExecuteGestureAtImage ("SwipeDown", imageName);
		}

		public void SwipeDownText (string imageName, PropertyList options = null) {
			ExecuteGestureAtText ("SwipeDown", imageName, options);
		}

		public void SwipeUp () {
			ExecuteGestureAtPoint ("SwipeUp");
		}

		public void SwipeUp (Point point) {
			ExecuteGestureAtPoint ("SwipeUp", point);
		}

		public void SwipeUp (string imageName) {
			ExecuteGestureAtImage ("SwipeUp", imageName);
		}

		public void SwipeUpText (string imageName, PropertyList options = null) {
			ExecuteGestureAtText ("SwipeUp", imageName, options);
		}

		public void PinchOut (
			double? durationSeconds = null, 
			int? distance = null) 
		{
			ExecutePinch (false, 
				durationSeconds, 
				distance);
		}

		public void PinchOut (
			Point atPoint, 
			double? durationSeconds = null,
			int? distance = null) 
		{
			ExecutePinch (false, 
				durationSeconds, 
				distance,
				atPoint);
		}

		public void PinchOut (
			string atImageName, 
			double? durationSeconds = null,
			int? distance = null) 
		{
			ExecutePinch (false, durationSeconds, 
				distance,
				SenseTalkStatementBuilder.Quote (atImageName));
		}

		public void PinchOut (
			Point atPoint,
			Point toPoint,
			double? durationSeconds = null) 
		{
			ExecutePinch (false, durationSeconds, 
				atPoint: atPoint,
				toPoint: toPoint);
		}

		public void PinchOut (
			string atImageName,
			string toImageName,
			double? durationSeconds = null) 
		{
			ExecutePinch (false, durationSeconds, 
				atPoint: SenseTalkStatementBuilder.Quote (atImageName),
				toPoint: SenseTalkStatementBuilder.Quote (toImageName));
		}

		public void PinchIn (
			double? durationSeconds = null, 
			int? distance = null) 
		{
			ExecutePinch (true, 
				durationSeconds, 
				distance);
		}

		public void PinchIn (
			Point atPoint, 
			double? durationSeconds = null,
			int? distance = null) 
		{
			ExecutePinch (true, 
				durationSeconds, 
				distance,
				atPoint);
		}

		public void PinchIn (
			string atImageName, 
			double? durationSeconds = null,
			int? distance = null) 
		{
			ExecutePinch (true, durationSeconds, 
				distance,
				SenseTalkStatementBuilder.Quote (atImageName));
		}

		public void PinchIn (
			Point atPoint,
			Point fromPoint,
			double? durationSeconds = null) 
		{
			ExecutePinch (true, durationSeconds, 
				atPoint: atPoint,
				fromPoint: fromPoint);
		}

		public void PinchIn (
			string atImageName,
			string fromImageName,
			double? durationSeconds = null) 
		{
			ExecutePinch (true, durationSeconds, 
				atPoint: SenseTalkStatementBuilder.Quote (atImageName),
				fromPoint: SenseTalkStatementBuilder.Quote (fromImageName));
		}

#endregion

#region Image searching

		public PropertyList ImageInfo (string imageName)
		{
			string[] imageNames = { imageName };
			var pLists = ImageInfo(imageNames); 
			return (pLists != null && pLists.Length > 0) ? pLists [0] : null;
		}

		public PropertyList[] ImageInfo (string[] imageNames)
		{
			var builder = new SenseTalkStatementBuilder ("ImageInfo", SenseTalkStatementType.Function);

			if (imageNames != null) foreach (string i in imageNames)
				builder.AddQuotedParameter (i);

			return Execute (builder.ToString ()).ReturnValueAsPropertyLists ();
		}

		public void WaitFor (string imageName, double? searchTimeoutSeconds)
		{
			Execute ((new SenseTalkStatementBuilder ("WaitFor"))
				.AddParameter (searchTimeoutSeconds)
				.AddQuotedParameter (imageName)
				.ToString());
		}

		public void WaitFor (string[] imageNames, double? searchTimeoutSeconds)
		{
			var builder = (new SenseTalkStatementBuilder ("WaitFor"))
				.AddParameter (searchTimeoutSeconds);

			foreach (var imageName in imageNames)
				builder.AddQuotedParameter (imageName);

			Execute (builder.ToString ());
		}

		public bool ImageFound (object imageNameOrNames, double? searchTimeoutSeconds = null)
		{
			var builder = new SenseTalkStatementBuilder ("ImageFound", SenseTalkStatementType.Function);
			var imageNames = imageNameOrNames as string[];
			var imageName = imageNameOrNames as string;

			builder.AddParameter (searchTimeoutSeconds);

			if (imageNames != null) foreach (string i in imageNames)
				builder.AddQuotedParameter (i);
			else if (imageName != null)
				builder.AddQuotedParameter (imageName);
			else
				builder.AddParameter (imageNameOrNames);
				
			return (Execute (builder.ToString ()).ReturnValue as bool?) == true;
		}
			
		public Point[] EveryImageLocation (string[] imageNames)
		{
			var builder = new SenseTalkStatementBuilder ("EveryImageLocation", SenseTalkStatementType.Function);

			if (imageNames != null) foreach (string i in imageNames)
				builder.AddQuotedParameter (i);

			return Execute (builder.ToString ()).ReturnValueAsPoints ();
		}

#endregion

#region SUT Text

		public void TypeText (string s)
		{
			Execute ((new SenseTalkStatementBuilder ("TypeText"))
				.AddQuotedParameter (s)
				.ToString());
		}

		public void TypeExpression (object expression)
		{
			Execute ((new SenseTalkStatementBuilder ("TypeText"))
				.AddParameter (expression)
				.ToString());
		}

		public string ReadText(Point point, PropertyList options = null)
		{
			return Execute ((new SenseTalkStatementBuilder ("ReadText", SenseTalkStatementType.Function))
				.AddParameter (point)
				.AddPropertyListParameters (options)
				.ToString()).ReturnValue as string;
		}

		public string ReadText(Rectangle rectangle, PropertyList options = null)
		{
			return Execute ((new SenseTalkStatementBuilder ("ReadText", SenseTalkStatementType.Function))
				.AddParameter (rectangle)
				.AddPropertyListParameters (options)
				.ToString()).ReturnValue as string;
		}

		public string ReadText(string imageName, PropertyList options = null)
		{
			return Execute ((new SenseTalkStatementBuilder ("ReadText", SenseTalkStatementType.Function))
				.AddParameter (imageName)
				.AddPropertyListParameters (options)
				.ToString()).ReturnValue as string;
		}

		public string ReadText(string[] imageNames, PropertyList options = null)
		{
			return Execute ((new SenseTalkStatementBuilder ("ReadText", SenseTalkStatementType.Function))
				.AddParameter (imageNames)
				.AddPropertyListParameters (options)
				.ToString()).ReturnValue as string;
		}

		public string RemoteClipboard(double? waitTimeoutSeconds = null)
		{
			return Execute ((new SenseTalkStatementBuilder ("RemoteClipboard", SenseTalkStatementType.Function))
				.AddParameter (waitTimeoutSeconds)
				.ToString()).ReturnValue as string;
		}

#endregion

#region Mobile device control

		public void LaunchApp (string appName)
		{
			Execute ((new SenseTalkStatementBuilder ("LaunchApp"))
				.AddQuotedParameter (appName)
				.ToString());
		}

		public void LaunchApp (string deviceName, string appName)
		{
			Execute ((new SenseTalkStatementBuilder ("LaunchApp"))
				.AddQuotedParameter (deviceName + " : " + appName)
				.ToString());
		}

#endregion

#region Private methods

		private void ExecuteGestureAtPoint(string gestureCommand, Point? point = null)
		{
			Execute ((new SenseTalkStatementBuilder (gestureCommand))
				.AddParameter (point)
				.ToString());
		}

		private void ExecuteGestureAtImage(string gestureCommand, string imageName, double? searchTimeoutSeconds = null)
		{
			Execute ((new SenseTalkStatementBuilder (gestureCommand))
				.AddQuotedPropertyListParameter ("Image", imageName)
				.AddPropertyListParameter ("WaitFor", searchTimeoutSeconds)
				.ToString());
		}

		private void ExecuteGestureAtText (string gestureCommand, string text, PropertyList options = null)
		{
			Execute ((new SenseTalkStatementBuilder (gestureCommand))
				.AddQuotedPropertyListParameter ("Text", text)
				.AddPropertyListParameters (options)
				.ToString());
		}


		private void ExecutePinch (
			bool isPinchIn,
			double? durationSeconds = null,
			int? distance = null,
			object atPoint = null,
			object fromPoint = null,
			object toPoint = null)
		{
			Execute ((new SenseTalkStatementBuilder (isPinchIn ? "PinchIn" : "PinchOut"))
				.AddPropertyListParameter ("At", atPoint)
				.AddPropertyListParameter ("Distance", distance)
				.AddPropertyListParameter ("From", fromPoint)
				.AddPropertyListParameter ("To", toPoint)
				.AddPropertyListParameter ("Duration", durationSeconds)
				.ToString ());
		}

#endregion

	}
}


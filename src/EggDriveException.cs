
namespace TestPlant.EggDriver
{

	public enum EggDriveFaultCode
	{
		NoError = 0,
		UnknownMethod = 1,
		SessionBusy = 2,
		NoActiveSession = 3,
		Exception = 4,
		SessionSuiteFailure = 5,
	}

	public class EggDriveException : System.ApplicationException
	{
		public EggDriveFaultCode FaultCode;

		public EggDriveException(EggDriveFaultCode faultCode, string message) :base(message) 
		{
			this.FaultCode = faultCode;
		}

		public EggDriveException(EggDriveFaultCode faultCode, string message, System.Exception inner) :base(message, inner) 
		{
			this.FaultCode = faultCode;
		}
	}

}


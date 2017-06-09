// Uncomment the following line to disable location tracking
 #define APP_METRICA_TRACK_LOCATION_DISABLED
// or just add APP_METRICA_TRACK_LOCATION_DISABLED into
// Player Settings -> Other Settings -> Scripting Define Symbols

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AppMetrica : MonoBehaviour
{
    private string APIKey;

    [SerializeField]
    private bool ExceptionsReporting = true;

    [SerializeField]
    private uint SessionTimeoutSec = 10;

    [SerializeField]
    private bool TrackLocation = true;

    [SerializeField]
    private bool LoggingEnabled = true;

    [SerializeField]
    private bool HandleFirstActivationAsUpdate = false;

    private static bool _isInitialized = false;
    private bool _actualPauseStatus = false;

    private static IYandexAppMetrica _metrica = null;
    private static object syncRoot = new Object ();

    public static IYandexAppMetrica Instance {
        get {
            if (_metrica == null) {
                lock (syncRoot) {
#if UNITY_IPHONE || UNITY_IOS
                    if (_metrica == null && Application.platform == RuntimePlatform.IPhonePlayer) {
                        _metrica = new YandexAppMetricaIOS ();
                    }
#elif UNITY_ANDROID
					if (_metrica == null && Application.platform == RuntimePlatform.Android) {
						_metrica = new YandexAppMetricaAndroid();
					}
#endif
                    if (_metrica == null) {
                        _metrica = new YandexAppMetricaDummy ();
                    }
                }
            }
            return _metrica;
        }
    }

    void SetupMetrica ()
    {
		#if UNITY_ANDROID
		APIKey = "c0cb879d-a63e-4eda-a975-e12ec060330e";
		#endif

		var configuration = new YandexAppMetricaConfig(APIKey) { 
			SessionTimeout = (int)SessionTimeoutSec,
			LoggingEnabled = LoggingEnabled,
		};

		#if !APP_METRICA_TRACK_LOCATION_DISABLED
		configuration.TrackLocationEnabled = TrackLocation;
		if (TrackLocation) {
		Input.location.Start ();
		}
		#endif

		CustomEventDelegate.ActionCustomEvent += OnCustomAnalyticsEvent;

		Instance.ActivateWithConfiguration(configuration);
		/*
        var configuration = new YandexAppMetricaConfig (APIKey) {
            SessionTimeout = (int)SessionTimeoutSec,
            LoggingEnabled = LoggingEnabled,
            HandleFirstActivationAsUpdateEnabled = HandleFirstActivationAsUpdate,
        };

#if !APP_METRICA_TRACK_LOCATION_DISABLED
        configuration.TrackLocationEnabled = TrackLocation;
        if (TrackLocation) {
            Input.location.Start ();
        }
#else
		configuration.TrackLocationEnabled = false;
#endif

        Instance.ActivateWithConfiguration (configuration);*/
        ProcessCrashReports ();
    }

    private void Awake ()
    {
        if (!_isInitialized) {
            _isInitialized = true;
            DontDestroyOnLoad (this.gameObject);
            SetupMetrica ();
        } else {
            Destroy (this.gameObject);
        }
    }

    private void Start ()
    {
        Instance.OnResumeApplication ();
    }

	private void OnCustomAnalyticsEvent(CustomEvent cEvent)
	{
		////Design event
		if (cEvent is CEAnalytics)
		{
			var aEvent = (CEAnalytics) cEvent;
			//логировние полученых параметров
			//Debug.Log("Analytic Design Event = " + aEvent.EventName);   

			string eventName = "Design";
			Dictionary<string, object> eventParams = new Dictionary<string, object> ();

			string paramKey = "";
			object paramValue = null;

			foreach (var key in aEvent.Parameters.Keys)
			{
				paramKey = key;
				aEvent.Parameters.TryGetValue (paramKey, out paramValue);
			}

			if (aEvent.IsHasEventValue)
			{
				if (paramValue != null)
				{
					//GameAnalytics.NewDesignEvent(aEvent.GetEventForSend(), aEvent.EventValue);
					eventParams.Add (string.Format("{0}", aEvent.EventName),
						new Dictionary<string, object> () {
						{ paramKey, new Dictionary<string, object> (){ { string.Format("{0}", paramValue), aEvent.EventValue } } }
					}
					);
				} else
				{
					eventParams.Add (string.Format("{0}",  aEvent.EventName), paramKey);
				}

				//Debug.Log("send GA DesignEvent = " + aEvent.GetEventForSend() + " Value = " + aEvent.EventValue);
			}
			else
			{
				//GameAnalytics.NewDesignEvent(aEvent.GetEventForSend());

				if (paramValue != null)
				{
					eventParams.Add (string.Format("{0}", aEvent.EventName),
						new Dictionary<string, object> () 
					{
						{ paramKey, paramValue}
					}
					);
				} else
				{
					eventParams.Add (string.Format("{0}", aEvent.EventName), paramKey);
				}
				//Debug.Log("send GA DesignEvent = " + aEvent.GetEventForSend());
			}  

			Instance.ReportEvent (eventName, eventParams);
		}

		if (cEvent is CEAnalyticsBusiness)
		{
			var aEvent = (CEAnalyticsBusiness)cEvent;

			string eventName = "Business";
			Dictionary<string, object> eventParams = new Dictionary<string, object> ();

			eventParams.Add (string.Format("{0}", aEvent.itemType), 
				new Dictionary<string, object>()
			{
				{aEvent.itemId, aEvent.amount / 100}
			} );

			//Debug.LogFormat("send GA Busines Event cartType = {0}; itemType = {1}; itemId = {2}; amount = {3}, currency = {4}", aEvent.cartType, aEvent.itemType, aEvent.itemId, aEvent.amount, aEvent.currency);

			Instance.ReportEvent (eventName, eventParams);
		}
	}

    private void OnEnable ()
    {
        if (ExceptionsReporting) {
#if UNITY_5
            Application.logMessageReceived += HandleLog;
#else
			Application.RegisterLogCallback(HandleLog);
#endif
        }
    }

    private void OnDisable ()
    {
        if (ExceptionsReporting) {
#if UNITY_5
            Application.logMessageReceived -= HandleLog;
#else
			Application.RegisterLogCallback(null);
#endif
        }
    }

    private void OnApplicationPause (bool pauseStatus)
    {
        if (_actualPauseStatus != pauseStatus) {
            _actualPauseStatus = pauseStatus;
            if (pauseStatus) {
                Instance.OnPauseApplication ();
            } else {
                Instance.OnResumeApplication ();
            }
        }
    }

    public void ProcessCrashReports ()
    {
        if (ExceptionsReporting) {
            var reports = CrashReport.reports;
            foreach (var report in reports) {
                var crashLog = string.Format ("Time: {0}\nText: {1}", report.time, report.text);
                Instance.ReportError ("Crash", crashLog);
                report.Remove ();
            }
        }
    }

    private void HandleLog (string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Exception) {
            Instance.ReportError (condition, stackTrace);
        }
    }

}

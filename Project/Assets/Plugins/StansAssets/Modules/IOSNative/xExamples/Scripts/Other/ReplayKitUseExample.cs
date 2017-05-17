////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReplayKitUseExample : BaseIOSFeaturePreview {

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	void Awake() {


		ISN_ReplayKit.ActionRecordStarted += HandleActionRecordStarted;
		ISN_ReplayKit.ActionRecordStoped += HandleActionRecordStoped;
		ISN_ReplayKit.ActionRecordInterrupted += HandleActionRecordInterrupted;

		ISN_ReplayKit.ActionShareDialogFinished += HandleActionShareDialogFinished;
		ISN_ReplayKit.ActionRecorderDidChangeAvailability += HandleActionRecorderDidChangeAvailability;

		IOSNativePopUpManager.showMessage ("Welcome", "Hey there, welcome to the ReplayKit testing scene!");

		ISN_Logger.Log("ReplayKit Is Avaliable: " + ISN_ReplayKit.Instance.IsAvailable);
	}



	void OnDestroy() {
		ISN_ReplayKit.ActionRecordStarted -= HandleActionRecordStarted;
		ISN_ReplayKit.ActionRecordStoped -= HandleActionRecordStoped;
		ISN_ReplayKit.ActionRecordInterrupted -= HandleActionRecordInterrupted;
	}

	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	void OnGUI() {
		
		UpdateToStartPos();
		
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Replay Kit", style);
		
		StartY+= YLableStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Start Recording")) {
			ISN_ReplayKit.Instance.StartRecording();
		}
		
		
		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Stop Recording")) {
			ISN_ReplayKit.Instance.StopRecording();
		}

		
	}

	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------

	void HandleActionRecordInterrupted (SA.Common.Models.Error error) {
		IOSNativePopUpManager.showMessage ("Video was interrupted with error: "," " + error.Message);
	}
	
	void HandleActionRecordStoped (SA.Common.Models.Result res) {
		if(res.IsSucceeded) {
			//the record is stopped, we can now show the sharing dialog.
			//you do not hae to show sharing dialog right after video was stopped
			//you can do this when user press "Share Replay" button in your game UI.
			ISN_ReplayKit.Instance.ShowVideoShareDialog();
		} else {
			IOSNativePopUpManager.showMessage ("Fail", "Error: " + res.Error.Message);
		}

	}

	void HandleActionShareDialogFinished (ReplayKitVideoShareResult res) {
		if(res.Sources.Length > 0) {
			foreach(string source in res.Sources) {
				IOSNativePopUpManager.showMessage ("Success", "User has shared the video to" + source);
			}
		} else {
			IOSNativePopUpManager.showMessage ("Fail", "User declined video sharing!");
		}
	}

	
	void HandleActionRecordStarted (SA.Common.Models.Result res) {
		if(res.IsSucceeded) {
			IOSNativePopUpManager.showMessage ("Success", "Record was successfully started!");

		} else {
			ISN_Logger.Log("Record start failed: " + res.Error.Message);
			IOSNativePopUpManager.showMessage ("Fail","Error: " + res.Error.Message);
		}
		ISN_ReplayKit.ActionRecordStarted -= HandleActionRecordStarted;
	}


	void HandleActionRecorderDidChangeAvailability (bool IsRecordingAvaliable) 	{
		ISN_Logger.Log("Is Recording Avaliable: " + IsRecordingAvaliable);

		ISN_ReplayKit.ActionRecordDiscard += HandleActionRecordDiscard;
		ISN_ReplayKit.Instance.DiscardRecording();
	}

	void HandleActionRecordDiscard () {
		ISN_Logger.Log("Record Discarded");
	}
	

	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

	

}

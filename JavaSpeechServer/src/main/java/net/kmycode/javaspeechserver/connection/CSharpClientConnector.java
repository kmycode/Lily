/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package net.kmycode.javaspeechserver.connection;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.FileReader;
import java.io.FileWriter;

/**
 *
 * @author KMY
 */
public class CSharpClientConnector {

	private static final String PIPE_NAME = "lily_speech";
	private static final String PIPE_NAME_CLIENT = "lily_speech_2";

	public static void initialize() {
		CSharpClientSender.getDefault();
		CSharpClientReceiver.getDefault();

		try {
			BufferedWriter server = new BufferedWriter(new FileWriter("\\\\.\\pipe\\" + PIPE_NAME));
			CSharpClientSender.getDefault().setServer(server);
			BufferedReader server2 = new BufferedReader(new FileReader("\\\\.\\pipe\\" + PIPE_NAME_CLIENT));
			CSharpClientReceiver.getDefault().setServer(server2);
		}
		catch (Exception e) {
			e.printStackTrace();
		}
	}

	private CSharpClientConnector() {
	}

}

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package net.kmycode.javaspeechserver;

import io.grpc.ManagedChannel;
import java.io.IOException;
import net.kmycode.javaspeechserver.audio.AudioRecorder;
import net.kmycode.javaspeechserver.cloud.StreamingRecognizeClient;
import net.kmycode.javaspeechserver.connection.CSharpClientSender;
import net.kmycode.javaspeechserver.connection.RecognitionResult;

/**
 *
 * @author KMY
 */
public class JavaSpeechServer {
	public static void main(String args[]) throws IOException, InterruptedException {
	    String host = "speech.googleapis.com";
		int port = 443;
		int sampling = 16000;

		AudioRecorder.initialize(sampling, sampling * AudioRecorder.BYTES_PER_SAMPLE / 10); // bytesPerBuffer: 100 ms

		ManagedChannel channel = StreamingRecognizeClient.createChannel(host, port);
		StreamingRecognizeClient client = new StreamingRecognizeClient(channel);

		final AudioRecorder recorder = AudioRecorder.getDefault();
		final CSharpClientSender sender = CSharpClientSender.getDefault();

		sender.send(new RecognitionResult(0, "Connection Succeed"));

		try {
			recorder.start();

			while (true) {
				if (recorder.preRead() && recorder.isSound()) {
					client.recognize();
				}
				Thread.sleep(100);
			}
		} finally {
			client.shutdown();
			recorder.stop();
			sender.close();
		}
	}
}

/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package net.kmycode.javaspeechserver.cloud;

import com.google.auth.oauth2.GoogleCredentials;
import com.google.cloud.speech.v1beta1.RecognitionConfig;
import com.google.cloud.speech.v1beta1.SpeechGrpc;
import com.google.cloud.speech.v1beta1.StreamingRecognitionConfig;
import com.google.cloud.speech.v1beta1.StreamingRecognitionResult;
import com.google.cloud.speech.v1beta1.StreamingRecognizeRequest;
import com.google.cloud.speech.v1beta1.StreamingRecognizeResponse;
import com.google.protobuf.ByteString;
import io.grpc.ManagedChannel;
import io.grpc.ManagedChannelBuilder;
import io.grpc.auth.ClientAuthInterceptor;
import io.grpc.stub.StreamObserver;
import java.io.FileInputStream;
import java.io.IOException;
import java.util.ArrayDeque;
import java.util.Arrays;
import java.util.List;
import java.util.Queue;
import java.util.concurrent.CountDownLatch;
import java.util.concurrent.Executors;
import java.util.concurrent.TimeUnit;
import net.kmycode.javaspeechserver.audio.AudioRecorder;
import net.kmycode.javaspeechserver.connection.CSharpClientSender;
import net.kmycode.javaspeechserver.connection.RecognitionResult;
import net.kmycode.javaspeechserver.connection.SoundInformation;
import org.apache.commons.lang3.time.StopWatch;
import org.apache.log4j.ConsoleAppender;
import static org.apache.log4j.ConsoleAppender.SYSTEM_OUT;
import org.apache.log4j.Level;
import org.apache.log4j.Logger;
import org.apache.log4j.SimpleLayout;

/**
 * Copy from https://github.com/GoogleCloudPlatform/java-docs-samples
 * speech/grpc/src/main/java/com/examples/cloud/speech/StreamingRecognizeClient.java
 * and some changes.
 */
public class StreamingRecognizeClient {

	private static final Logger logger = Logger.getLogger(StreamingRecognizeClient.class.getName());
	private static final List<String> OAUTH2_SCOPES = Arrays.asList("https://www.googleapis.com/auth/cloud-platform");
	private static final String CREDENTIAL_PATH = ".\\credential\\SpeechTest-d2316e38bee3.json";
	private static int messageId = 1;

	private final AudioRecorder recorder = AudioRecorder.getDefault();
	private final CSharpClientSender sender = CSharpClientSender.getDefault();

	private StreamObserver<StreamingRecognizeRequest> requestObserver;
	private int notSoundCount = 0;
	private final ManagedChannel channel;
	private final SpeechGrpc.SpeechStub speechClient;
	private final Queue<ByteString> byteStringQueue = new ArrayDeque<>();

	/**
	 * Construct client connecting to Cloud Speech server at {@code host:port}.
	 */
	public StreamingRecognizeClient(ManagedChannel channel)
			throws IOException {
		this.channel = channel;

		speechClient = SpeechGrpc.newStub(channel);

		// Send log4j logs to Console
		// If you are going to run this on GCE, you might wish to integrate with
		// google-cloud-java logging. See:
		// https://github.com/GoogleCloudPlatform/google-cloud-java/blob/master/README.md#stackdriver-logging-alpha
		ConsoleAppender appender = new ConsoleAppender(new SimpleLayout(), SYSTEM_OUT);
		logger.addAppender(appender);
	}

	public void shutdown() throws InterruptedException {
		channel.shutdown().awaitTermination(5, TimeUnit.SECONDS);
	}

	public static ManagedChannel createChannel(String host, int port) throws IOException {
		//GoogleCredentials creds = GoogleCredentials.getApplicationDefault();
		GoogleCredentials creds = GoogleCredentials.fromStream(new FileInputStream(CREDENTIAL_PATH));
		creds = creds.createScoped(OAUTH2_SCOPES);
		ManagedChannel channel =
				ManagedChannelBuilder.forAddress(host, port)
						.intercept(new ClientAuthInterceptor(creds, Executors.newSingleThreadExecutor()))
						.build();

		return channel;
	}

	private static int newId() {
		return messageId++;
	}

	/** Send streaming recognize requests to server. */
	public void recognize() throws InterruptedException, IOException {
		final StopWatch stopwatch = new StopWatch();
		final int messageId = newId();

		final CountDownLatch finishLatch = new CountDownLatch(1);
		StreamObserver<StreamingRecognizeResponse> responseObserver =
				new StreamObserver<StreamingRecognizeResponse>() {
					private int sentenceLength = 1;
					/**
					 * Prints the transcription results. Interim results are overwritten by subsequent
					 * results, until a final one is returned, at which point we start a new line.
					 *
					 * Flags the program to exit when it hears "exit".
					 */
					@Override
					public void onNext(StreamingRecognizeResponse response) {

						byteStringQueue.clear();
						stopwatch.reset();

						List<StreamingRecognitionResult> results = response.getResultsList();
						if (results.size() < 1) {
							return;
						}

						StreamingRecognitionResult result = results.get(0);
						String transcript = result.getAlternatives(0).getTranscript();

						// Print interim results with a line feed, so subsequent transcriptions will overwrite
						// it. Final result will print a newline.
						String format = "%-" + this.sentenceLength + 's';
						format += " (" + result.getAlternatives(0).getConfidence() + ") ";
						if (result.getIsFinal()) {
							format += '\n';
							this.sentenceLength = 1;
							finishLatch.countDown();
						} else {
							//format += '\r';
							format += '\n';
							this.sentenceLength = transcript.length();
						}
						System.out.print(String.format(format, transcript));

						RecognitionResult resultData = new RecognitionResult(messageId, transcript);
						StreamingRecognizeClient.this.sender.send(resultData);
					}

					@Override
					public void onError(Throwable error) {
						logger.log(Level.ERROR, "recognize failed: {0}", error);
						finishLatch.countDown();
					}

					@Override
					public void onCompleted() {
						logger.info("recognize completed.");
						finishLatch.countDown();
					}
				};

		this.requestObserver = this.speechClient.streamingRecognize(responseObserver);
		try {
			// Build and send a StreamingRecognizeRequest containing the parameters for
			// processing the audio.
			RecognitionConfig config =
					RecognitionConfig.newBuilder()
							.setEncoding(RecognitionConfig.AudioEncoding.LINEAR16)
							.setSampleRate(recorder.getSamplingRate())
							.setLanguageCode("ja-JP")
							.build();
			StreamingRecognitionConfig streamingConfig =
					StreamingRecognitionConfig.newBuilder()
							.setConfig(config)
							.setInterimResults(true)
							.setSingleUtterance(false)
							.build();

			StreamingRecognizeRequest initial =
					StreamingRecognizeRequest.newBuilder().setStreamingConfig(streamingConfig).build();
			requestObserver.onNext(initial);

			while (this.byteStringQueue.size() > 0) {
				ByteString data = this.byteStringQueue.poll();
				this.request(data);
			}

			// Read and send sequential buffers of audio as additional RecognizeRequests.
			while (finishLatch.getCount() > 0 && recorder.read()) {
				this.sender.send(new SoundInformation(this.recorder.getVolume()));

				ByteString data = this.recorder.getBufferAsByteString();
				this.byteStringQueue.add(data);

				if (!stopwatch.isStarted()) {
					stopwatch.start();
				}
				else if (stopwatch.getTime() > 2000) {
					this.byteStringQueue.clear();
					break;
				}

				this.request(data);

				if (!recorder.isSound()) {
					this.notSoundCount++;
					if (this.notSoundCount >= 5) {
						// stop recognizition
						break;
					}
				}
			}
		} catch (RuntimeException e) {
			// Cancel RPC.
			requestObserver.onError(e);
			throw e;
		}
		// Mark the end of requests.
		requestObserver.onCompleted();

		// Receiving happens asynchronously.
		finishLatch.await(1, TimeUnit.MINUTES);
	}

	private void request(ByteString data) {
		StreamingRecognizeRequest request =
				StreamingRecognizeRequest.newBuilder()
						.setAudioContent(data)
						.build();
		this.requestObserver.onNext(request);
		this.notSoundCount = 0;
	}
}

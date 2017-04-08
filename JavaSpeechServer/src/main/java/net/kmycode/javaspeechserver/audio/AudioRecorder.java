/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package net.kmycode.javaspeechserver.audio;

import com.google.protobuf.ByteString;
import javax.sound.sampled.AudioFormat;
import javax.sound.sampled.AudioSystem;
import javax.sound.sampled.DataLine;
import javax.sound.sampled.LineUnavailableException;
import javax.sound.sampled.TargetDataLine;

/**
 *
 * @author KMY
 */
public class AudioRecorder {

	protected TargetDataLine mockDataLine = null;
	private boolean isRecording = false;
	private byte[] buffer = null;
	private int bytesReadFromRecording;
	private boolean isPreRead = false;
	private boolean preReadResult;

	public static final int BYTES_PER_SAMPLE = 2; // bytes per sample for LINEAR16

	private int samplingRate;
	public int getSamplingRate() {
		return this.samplingRate;
	}

	private int bytesPerBuffer; // buffer size in bytes
	public int getBytesPerBuffer() {
		return this.bytesPerBuffer;
	}

	private static AudioRecorder defaultInstance;
	public static AudioRecorder getDefault() {
		if (defaultInstance == null) {
			throw new RuntimeException("AudioRecorder is not initialized.");
		}
		return defaultInstance;
	}
	public static void initialize(int samplingRate, int bytesPerBuffer) {
		defaultInstance = new AudioRecorder();
		defaultInstance.bytesPerBuffer = bytesPerBuffer;
		defaultInstance.buffer = new byte[bytesPerBuffer];
		defaultInstance.samplingRate = samplingRate;
	}

	/**
	 * Return a Line to the audio input device.
	 */
	public TargetDataLine getAudioInputLine() {
		// For testing
		if (null != mockDataLine) {
			return mockDataLine;
		}

		AudioFormat format = new AudioFormat(this.samplingRate, BYTES_PER_SAMPLE * 8, 1, true, false);
		DataLine.Info info = new DataLine.Info(TargetDataLine.class, format);
		if (!AudioSystem.isLineSupported(info)) {
			throw new RuntimeException(String.format(
						"Device doesn't support LINEAR16 mono raw audio format at {%d}Hz", this.samplingRate));
		}
		try {
			TargetDataLine line = (TargetDataLine) AudioSystem.getLine(info);
			// Make sure the line buffer doesn't overflow while we're filling this thread's buffer.
			line.open(format, bytesPerBuffer * 5);
			this.mockDataLine = line;
			return line;
		} catch (LineUnavailableException e) {
			throw new RuntimeException(e);
		}
	}

	public void start() {
		if (this.isRecording) {
			return;
		}

		this.getAudioInputLine().start();
		this.isRecording = true;
	}

	public boolean preRead() {
		this.isPreRead = true;
		this.preReadResult = this.readPrivate();
		return this.preReadResult;
	}

	public boolean read() {
		if (this.isPreRead) {
			this.isPreRead = false;
			return this.preReadResult;
		}

		return this.readPrivate();
	}

	private boolean readPrivate() {
		this.bytesReadFromRecording = this.getAudioInputLine().read(this.buffer, 0, this.buffer.length);
		return this.bytesReadFromRecording != -1;
	}

	public boolean isSound() {
		return this.getVolume() > 0.0003;
	}

	public float getVolume() {
		float sum = 0;
		int count = 0;

		for (int i = 0; i < this.buffer.length; i += 2) {
			byte h = this.buffer[i + 1];
			byte l = this.buffer[i];

			short valueInteger = (short) ((h << 8) | l);
			if (valueInteger < 0) valueInteger *= -1;
			float value = (float)(int)valueInteger / 65536;

			sum += value;
			count++;
		}

		return sum / count;
	}

	public ByteString getBufferAsByteString() {
		return ByteString.copyFrom(this.buffer, 0, this.bytesReadFromRecording);
	}

	public void stop() {
		if (!this.isRecording) {
			return;
		}

		this.getAudioInputLine().stop();
		this.isRecording = false;
	}

}

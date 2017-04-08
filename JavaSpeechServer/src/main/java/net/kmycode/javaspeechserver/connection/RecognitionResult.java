/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package net.kmycode.javaspeechserver.connection;

/**
 * 音声認識結果
 * @author KMY
 */
public class RecognitionResult {

	private int id;
	private String text;

	public RecognitionResult(int id, String text) {
		this.id = id;
		this.text = text;
	}

}

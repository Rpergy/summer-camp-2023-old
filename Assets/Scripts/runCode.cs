using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;

public class runCode : MonoBehaviour
{
    public Transform robot;
    public TextMeshProUGUI code;
    public TextMeshProUGUI error;

    public float moveSpeed = 0.1f;
    public float turnSpeed = 1f;
    public float timeScale = 0.03f;

    public bool trail = false;

    Vector3 startPos;
    Vector3 startRot;

    public void Start() {
        startPos = robot.position;
        startRot = robot.eulerAngles;
    }

    public void reset() {
        robot.position = startPos;
        robot.rotation = Quaternion.Euler(startRot);
        StopAllCoroutines();
        trail = false;
    }

    public void interpret() {
        string userCode = code.text;
        string[] lines = userCode.Split(";");

        error.text = "";
        reset();

        StopAllCoroutines();
        trail = false;
        StartCoroutine(Movement(lines.SkipLast(1).ToArray()));
    }

    IEnumerator Movement(string[] lines) {
        trail = true;
        for (int i = 0; i < lines.Length; i++) {
            string[] command;
            if (i == 0) {
                command = lines[i].Split(" ");
            }
            else {
                command = lines[i].Split(" ");
                command[0] = command[0].Substring(1);
            }

            Debug.Log("Command: " + String.Join(" ", command));

            if (command[0] == "move") {
                int moveAmt = 0;
                try {
                    moveAmt = Int32.Parse(command[1]);
                }
                catch {
                    error.text = "Error: Non-Integer movement amount on line " + (i + 1);
                    break;
                }
                
                if (moveAmt > 0) {
                    for (float j = 0; j <= moveAmt; j += moveSpeed) {
                        robot.Translate(new Vector3(moveSpeed, 0, 0), Space.Self);
                        yield return new WaitForSeconds(timeScale);
                    }
                }
                else if (moveAmt < 0) {
                    for (float j = 0; j >= moveAmt; j -= moveSpeed) {
                        robot.Translate(new Vector3(-moveSpeed, 0, 0), Space.Self);
                        yield return new WaitForSeconds(timeScale);
                    }
                }
            }
            else if (command[0] == "strafe") {
                int moveAmt = 0;
                try {
                    moveAmt = Int32.Parse(command[1]);
                }
                catch {
                    error.text = "Error: Non-Integer movement amount on line " + (i + 1);
                    break;
                }
                
                if (moveAmt > 0) {
                    for (float j = 0; j <= moveAmt; j += moveSpeed) {
                        robot.Translate(new Vector3(0, 0, moveSpeed), Space.Self);
                        yield return new WaitForSeconds(timeScale);
                    }
                }
                else if (moveAmt < 0) {
                    for (float j = 0; j >= moveAmt; j -= moveSpeed) {
                        robot.Translate(new Vector3(0, 0, -moveSpeed), Space.Self);
                        yield return new WaitForSeconds(timeScale);
                    }
                }
            }
            else if (command[0] == "turn") {
                int angle = 0;
                try {
                    angle = Int32.Parse(command[1]);
                }
                catch {
                    error.text = "Error: Non-Integer movement amount on line " + (i + 1);
                    break;
                }
                Vector3 startingRot = robot.eulerAngles;
                Vector3 endingRot = new Vector3(robot.eulerAngles.x, robot.eulerAngles.y + angle, robot.eulerAngles.z);

                for (float j = 0; j <= 1; j += turnSpeed) {
                    robot.eulerAngles = Vector3.Lerp(startingRot, endingRot, j);
                    yield return new WaitForSeconds(timeScale);
                }
                robot.eulerAngles = endingRot;
            }
            else if (command[0] == "endloop") {
                // Loop backwards from endloop line to find a loop command: if the loop number is 0, break from the loop statement
                int loopLineIndex = -1;
                bool finishedLoop = false;

                for (int j = i; j >= 0; j--) {
                    string[] cmd = lines[j].Split(" ");
                    if (cmd[0] == "loop") {
                        if (cmd[1] == "1") {
                            finishedLoop = true;
                            loopLineIndex = -2;
                        }
                        else {
                            loopLineIndex = j;
                        }
                        break;
                    }
                }

                if (loopLineIndex == -1) { // When we havent found a starting loop
                    error.text = "Error: Ending loop used without matching starting loop on line " + i;
                }

                if (!finishedLoop) {
                    // Decrement the loop number by one
                    int loopAmt;
                    Debug.Log("loop number: " + lines[loopLineIndex].Split(" ")[1]);
                    try {
                        loopAmt = Int32.Parse(lines[loopLineIndex].Split(" ")[1]);
                    }
                    catch {
                        error.text = "Error: Non-Integer loop amount on line " + (i + 1);
                        break;
                    }
                    lines[loopLineIndex] = "loop " + (loopAmt - 1);

                    // Set i to the line after the loop command
                    i = loopLineIndex;
                }
            }
            else if (command[0] != "loop") {
                error.text = "Error: Unrecognized command on line " + (i + 1);
            }
        }
        trail = false;
        yield return null;
    }
}

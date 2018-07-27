'use strict';

var API_KEY = 'f60616efeeb24ee4a5ba764b13f7c658';

setInterval(timerFire, 30000);

function timerFire() {
    //remoteVideo
    getFrame(function(frameData) {
        getEmotion(frameData);
    });
}

function getFrame(callback) {
    console.log('grabbing frame for emotion detection');
    var canvas = document.querySelector('.screenshotCanvas');

    // Grab a frame from the video -> e.g. for cognitive services   
    var context = canvas.getContext('2d');
    context.drawImage(localVideo, 0, 0, 220, 150);

    canvas.toBlob(callback)
}

function getEmotion(data) {
    return fetch('https://northeurope.api.cognitive.microsoft.com/face/v1.0/detect?returnFaceAttributes=emotion',
        {   method: 'POST',
            headers: {
                'Ocp-Apim-Subscription-Key': API_KEY,
                'Content-Type': 'application/octet-stream' },
            body: data })
    .then(function(response) {
        return response.json();
    })
    .then(function(json) {
        console.log('Emotion response: ' + JSON.stringify(json[0].scores));

        var resp = JSON.parse(JSON.stringify(json));
        var emotions = resp[0].faceAttributes.emotion;

        let max;
        for (var prop in emotions)
        {
            if (max == null) {
                max = prop;
                continue;
            }
            if (emotions[prop] > emotions[max]) {
                max = prop;
            }
        }

        //addEmotion();

        console.log('Most likely emotion: ' + max);

    });
}

function addEmotion() {
    var par = document.getElementById("emotionText");
    par.className += "fa-smile-o";
}
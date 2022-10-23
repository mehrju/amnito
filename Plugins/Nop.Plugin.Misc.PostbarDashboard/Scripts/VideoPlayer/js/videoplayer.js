//get mouse parent position
function getPositionData(el){
  var xPosition = 0;
  var yPosition = 0;
  while (el) {
    if (el.tagName == "BODY") {
      // deal with browser quirks with body/window/document and page scroll
      var xScrollPos = el.scrollLeft || document.documentElement.scrollLeft;
      var yScrollPos = el.scrollTop || document.documentElement.scrollTop;
 
      xPosition += (el.offsetLeft - xScrollPos + el.clientLeft);
      yPosition += (el.offsetTop - yScrollPos + el.clientTop);
    } else {
      xPosition += (el.offsetLeft - el.scrollLeft + el.clientLeft);
      yPosition += (el.offsetTop - el.scrollTop + el.clientTop);
    }
 
    el = el.offsetParent;
  }
  return {
    x: xPosition,
    y: yPosition
  };
}
//video player class
function videoPlayer(){
	//the parent element
	videoPlayer.prototype.elem=null;
	//the video tag element
	videoPlayer.prototype.video=null;
	//the src of video
	videoPlayer.prototype.file=null;
	//video player cover
	videoPlayer.prototype.cover=null;
	//video player buttons
	videoPlayer.prototype.play_btn=null;
	videoPlayer.prototype.tex_current=null;
	videoPlayer.prototype.timeline_btn=null;
	videoPlayer.prototype.text_duration=null;
	videoPlayer.prototype.mute_btn=null;
	videoPlayer.prototype.expand_btn=null;

	//starts the video player
	this.init=function(elem){
		//add new function to window for element event listener
		window.listenElem=function(event,obj,func){
			obj.addEventListener(event,func,false);
		}
		//add new function to window for elements event listener
		window.listenElems=function(event,objs,func){
			var len=objs.length;
			for(var i=0;i<len;i++){
				objs[i].addEventListener(event,func,false);
			}
		}

		//set parent elem for video player
		videoPlayer.elem=elem;
		//set video cover
		videoPlayer.cover=elem.getElementsByClassName("videoPlayer_cover")[0];
		//set video elem
		videoPlayer.video=elem.getElementsByClassName("videoPlayer_video")[0];
		videoPlayer.file=videoPlayer.video.src;
		var videoPlayer_buttons=elem.querySelectorAll(".videoPlayer_buttons ul li");		
		//find buttons and put them into vars
		for(var i=0;i<videoPlayer_buttons.length;i++){
			switch(videoPlayer_buttons[i].getAttribute("data-button")){
				case "play":
					videoPlayer.play_btn=videoPlayer_buttons[i];
					break;
				case "current":
					videoPlayer.text_current=videoPlayer_buttons[i];
					break;
				case "timeline":
					videoPlayer.timeline_btn=videoPlayer_buttons[i];
					break;
				case "duration":
					videoPlayer.text_duration=videoPlayer_buttons[i];
					break;
				case "mute":
					videoPlayer.mute_btn=videoPlayer_buttons[i];
					break;
				case "fullscreen":
					videoPlayer.expand_btn=videoPlayer_buttons[i];
					break;
				default:
					console.warn("a btn is not valid");
					break;
			}
		}
		
		/***
		add some event listeners for controling every thing:
		***/
		
		//add event listener for video cover
		window.listenElem("click",videoPlayer.cover,this.videoCoverClicked);
		
		//add event listener for play button on first page
		window.listenElems("click",videoPlayer_buttons,this.buttonsClicked);
		
		//add event for updating time label when music is playing
		window.listenElem("timeupdate",videoPlayer.video,this.updatePlayerTime);
		
		//add event listener for player when music had been ended
		window.listenElem("ended",videoPlayer.video,this.endPlayerTime);
		
		//add event listener for player when music had been paused
		window.listenElem("play",videoPlayer.video,this.playedPlayer);
		
		//add event listener for player when music had been played
		window.listenElem("pause",videoPlayer.video,this.pausedPlayer);
		
		//add event listener for progressing
		window.listenElem("progress",videoPlayer.video,this.progressPlayer);
		
		//add event listener for loading meta data
		window.listenElem("loadedmetadata",videoPlayer.video,this.progressPlayer);
	
		//add event listener for volume change

		window.listenElem("volumechange",videoPlayer.video,this.volumechange);	

		//add event listener for error on music player
		window.listenElem("error",videoPlayer.video,this.errorPlayer);
	}

	/*functions for control actions*/

	this.videoCoverClicked=function(event){
		alert('1');
		event.currentTarget.style.display="none";
		if(videoPlayer.video===undefined || typeof videoPlayer.video===null){
			console.warn("cant find video tag with class='videoPlayer_video'");
		}else{
			videoPlayer.video.play();
		}
		event.preventDefault();
		return false;
	};

	this.buttonsClicked=function(event){
		
		switch(event.currentTarget.getAttribute("data-button")){
			case "play":
				console.log("play");
				if(videoPlayer.video.paused){
					if(videoPlayer.video.duration > 0){
						videoPlayer.video.play();
					}else{
						videoPlayer.play(videoPlayer.file);
					}
				}else{
					if(videoPlayer.video.duration > 0){
						videoPlayer.video.pause();
					}else{
						videoPlayer.play(videoPlayer.file);
					}
				}
				break;
			case "timeline":
				alert('کاربر گرامی شما نمیتوانید ویدئو را به جلو ببرید');
				// if(videoPlayer.video.duration>0){
				// 	var parentPosition=getPositionData(event.currentTarget);
				// 	var mousex = event.clientX - parentPosition.x;
				// 	var timelineWidth=parseFloat(videoPlayer.timeline_btn.offsetWidth);
				// 	var newCurrentTime=(parseFloat(mousex)*parseFloat(videoPlayer.video.duration))/parseFloat(timelineWidth);
				// 	console.log(mousex+"/"+timelineWidth);
				// 	videoPlayer.video.currentTime=newCurrentTime;
				// }
				// ;
				break;
			case "mute":
				console.log('volume');
				if(videoPlayer.video.muted===false){
					videoPlayer.video.muted=true;
					event.currentTarget.className="fa fa-volume-off fa-lg";
				}else{
					videoPlayer.video.muted=false;
					event.currentTarget.className="fa fa-volume-up fa-lg";
				}
				break;
			case "fullscreen":
				if (!document.fullscreenElement && !document.mozFullScreenElement && !document.webkitFullscreenElement && !document.msFullscreenElement ) {
					event.currentTarget.className="fa fa-compress fa-lg";
					if (videoPlayer.elem.requestFullscreen){
						videoPlayer.elem.requestFullscreen();
					}else if(videoPlayer.elem.msRequestFullscreen){
						videoPlayer.elem.msRequestFullscreen();
					}else if(videoPlayer.elem.mozRequestFullScreen){
						videoPlayer.elem.mozRequestFullScreen();
					}else if(videoPlayer.elem.webkitRequestFullscreen){
					 	videoPlayer.elem.webkitRequestFullscreen(Element.ALLOW_KEYBOARD_INPUT);
					}else{
						event.currentTarget.className="fa fa-expand fa-lg";
						alert('مرورگر شما قابلیت تمام صفحه شدن ویدئو را پشتیبانی نمیکند');
					}
				} else {
				  if (document.exitFullscreen) {
				    document.exitFullscreen();
				  } else if (document.msExitFullscreen) {
				    document.msExitFullscreen();
				  } else if (document.mozCancelFullScreen) {
				    document.mozCancelFullScreen();
				  } else if (document.webkitExitFullscreen) {
				    document.webkitExitFullscreen();
				  }
				  event.currentTarget.className="fa fa-expand fa-lg";
				}
				break;
			default:
				console.warn("the button you have clicked is not valid");
				break;
		}
	}

	this.updatePlayerTime=function(event){
		
			var minutes = Math.floor(parseInt(this.currentTime) / 60);
			var seconds = parseInt(this.currentTime) - (minutes * 60);
			var show=(minutes<10?'0'+minutes:minutes)+':'+(seconds<10?'0'+seconds:seconds);
			if(isNaN(this.currentTime)){show="00:00";}
			videoPlayer.text_current.innerHTML=show;	
			minutes = Math.floor(parseInt(this.duration) / 60);
			seconds = parseInt(this.duration) - (minutes * 60);
			show=(minutes<10?'0'+minutes:minutes)+':'+(seconds<10?'0'+seconds:seconds);
			if(!isNaN(this.duration)){
				videoPlayer.text_duration.innerHTML=show;
			}
			//timeline move:
			var timeline=videoPlayer.timeline_btn;
			var timelineWidth=parseFloat(timeline.offsetWidth);
			// var playedPercent=parseInt(parseFloat(this.currentTime*100)/parseFloat(this.duration));
			var playedWidth=parseFloat(parseFloat(this.currentTime)*parseFloat(timelineWidth)/parseFloat(this.duration));
			timeline.firstElementChild.firstElementChild.style.width=playedWidth+"px";	
	};

	this.endPlayerTime=function(event){
		alert('4');
		videoPlayer.play_btn.className="fa fa-play fa-lg";
		videoPlayer.video.currentTime=0;
	};

	this.playedPlayer=function(event){
		alert('5');
		videoPlayer.play_btn.className="fa fa-pause fa-lg";
		videoPlayer.cover.style.display="none";
	};

	this.pausedPlayer=function(event){
		alert('6');
		videoPlayer.play_btn.className="fa fa-play fa-lg";
	};

	this.progressPlayer=function(event){
		alert('7');
		//timeline move:
		var timeline=videoPlayer.timeline_btn;
		var timelineWidth=parseFloat(timeline.offsetWidth);
		if(this.buffered.length>0){
			var bufferedPercent=parseFloat(parseFloat(parseFloat(this.buffered.end(this.buffered.length-1))*100)/parseFloat(this.duration));
			var bufferedWidth=parseFloat(parseFloat(this.buffered.end(this.buffered.length-1))*parseFloat(timelineWidth)/parseFloat(this.duration));
			timeline.firstElementChild.style.width=bufferedPercent+"%";
		}else{
			timeline.firstElementChild.style.width="100%";
		}
	};

	this.volumechange=function(event){
		alert('8');
		if(videoPlayer.video.muted===true){
			videoPlayer.mute_btn.className="fa fa-volume-off fa-lg";
		}else{
			videoPlayer.mute_btn.className="fa fa-volume-up fa-lg";
		}
	}

	this.errorPlayer=function(event){
		alert('9');
		videoPlayer.play_btn.className="fa fa-warning fa-lg";
	};
}

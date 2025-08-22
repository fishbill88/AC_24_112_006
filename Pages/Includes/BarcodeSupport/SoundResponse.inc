<script language="javascript" type="text/javascript">
	function ActionCallback(callbackContext) {
		if ((callbackContext.info.name.toLowerCase().startsWith("scan") || callbackContext.info.name == "ElapsedTime") && callbackContext.control.longRunInProcess == null) {
			var edInfoMessageSoundFile = px_alls["edInfoMessageSoundFile"];
			if (edInfoMessageSoundFile) {
				px.playSound(edInfoMessageSoundFile.getValue());
			}
		}
	}

	window.addEventListener('load', function () { px_callback.addHandler(ActionCallback); });
</script>
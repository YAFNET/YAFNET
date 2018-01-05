$(function() {
$.fn.emojiPicker.emojis = [
  {
    "name": "grinning",
    "unicode": {"apple":"1F600", "google":"1F600", "twitter":"1F600"},
    "shortcode": "grinning",
    "description": "GRINNING FACE",
    "category": "people"
  },
  {
    "name": "grin",
    "unicode": {"apple":"1F601", "google":"1F601", "twitter":"1F601"},
    "shortcode": "grin",
    "description": "GRINNING FACE WITH SMILING EYES",
    "category": "people"
  },
  {
    "name": "grimacing",
    "unicode": {"apple":"1F62C", "google":"1F62C", "twitter":"1F62C"},
    "shortcode": "grimacing",
    "description": "GRIMACING FACE",
    "category": "people"
  },
  {
    "name": "joy",
    "unicode": {"apple":"1F602", "google":"1F602", "twitter":"1F602"},
    "shortcode": "joy",
    "description": "FACE WITH TEARS OF JOY",
    "category": "people"
  },
  {
    "name": "smiley",
    "unicode": {"apple":"1F603", "google":"1F603", "twitter":"1F603"},
    "shortcode": "smiley",
    "description": "SMILING FACE WITH OPEN MOUTH",
    "category": "people"
  },
  {
    "name": "smile",
    "unicode": {"apple":"1F604", "google":"1F604", "twitter":"1F604"},
    "shortcode": "smile",
    "description": "SMILING FACE WITH OPEN MOUTH AND SMILING EYES",
    "category": "people"
  },
  {
    "name": "sweat_smile",
    "unicode": {"apple":"1F605", "google":"1F605", "twitter":"1F605"},
    "shortcode": "sweat_smile",
    "description": "SMILING FACE WITH OPEN MOUTH AND COLD SWEAT",
    "category": "people"
  },
  {
    "name": "laughing",
    "unicode": {"apple":"1F606", "google":"1F606", "twitter":"1F606"},
    "shortcode": "laughing",
    "description": "SMILING FACE WITH OPEN MOUTH AND TIGHTLY-CLOSED EYES",
    "category": "people"
  },
  {
    "name": "innocent",
    "unicode": {"apple":"1F607", "google":"1F607", "twitter":"1F607"},
    "shortcode": "innocent",
    "description": "SMILING FACE WITH HALO",
    "category": "people"
  },
  {
    "name": "wink",
    "unicode": {"apple":"1F609", "google":"1F609", "twitter":"1F609"},
    "shortcode": "wink",
    "description": "WINKING FACE",
    "category": "people"
  },
  {
    "name": "blush",
    "unicode": {"apple":"1F60A", "google":"1F60A", "twitter":"1F60A"},
    "shortcode": "blush",
    "description": "SMILING FACE WITH SMILING EYES",
    "category": "people"
  },
  {
    "name": "slightly_smiling_face",
    "unicode": {"apple":"1F642", "google":"1F642", "twitter":"1F642"},
    "shortcode": "slightly_smiling_face",
    "description": "slightly smiling face",
    "category": "people"
  },
  {
    "name": "upside_down_face",
    "unicode": {"apple":"1F643", "google":"1F643", "twitter":"1F643"},
    "shortcode": "upside_down_face",
    "description": "upside-down face",
    "category": "people"
  },
  {
    "name": "relaxed",
    "unicode": {"apple":"263A", "google":"263A", "twitter":"263A"},
    "shortcode": "relaxed",
    "description": "WHITE SMILING FACE",
    "category": "people"
  },
  {
    "name": "yum",
    "unicode": {"apple":"1F60B", "google":"1F60B", "twitter":"1F60B"},
    "shortcode": "yum",
    "description": "FACE SAVOURING DELICIOUS FOOD",
    "category": "people"
  },
  {
    "name": "relieved",
    "unicode": {"apple":"1F60C", "google":"1F60C", "twitter":"1F60C"},
    "shortcode": "relieved",
    "description": "RELIEVED FACE",
    "category": "people"
  },
  {
    "name": "heart_eyes",
    "unicode": {"apple":"1F60D", "google":"1F60D", "twitter":"1F60D"},
    "shortcode": "heart_eyes",
    "description": "SMILING FACE WITH HEART-SHAPED EYES",
    "category": "people"
  },
  {
    "name": "kissing_heart",
    "unicode": {"apple":"1F618", "google":"1F618", "twitter":"1F618"},
    "shortcode": "kissing_heart",
    "description": "FACE THROWING A KISS",
    "category": "people"
  },
  {
    "name": "kissing",
    "unicode": {"apple":"1F617", "google":"1F617", "twitter":"1F617"},
    "shortcode": "kissing",
    "description": "KISSING FACE",
    "category": "people"
  },
  {
    "name": "kissing_smiling_eyes",
    "unicode": {"apple":"1F619", "google":"1F619", "twitter":"1F619"},
    "shortcode": "kissing_smiling_eyes",
    "description": "KISSING FACE WITH SMILING EYES",
    "category": "people"
  },
  {
    "name": "kissing_closed_eyes",
    "unicode": {"apple":"1F61A", "google":"1F61A", "twitter":"1F61A"},
    "shortcode": "kissing_closed_eyes",
    "description": "KISSING FACE WITH CLOSED EYES",
    "category": "people"
  },
  {
    "name": "stuck_out_tongue_winking_eye",
    "unicode": {"apple":"1F61C", "google":"1F61C", "twitter":"1F61C"},
    "shortcode": "stuck_out_tongue_winking_eye",
    "description": "FACE WITH STUCK-OUT TONGUE AND WINKING EYE",
    "category": "people"
  },
  {
    "name": "stuck_out_tongue_closed_eyes",
    "unicode": {"apple":"1F61D", "google":"1F61D", "twitter":"1F61D"},
    "shortcode": "stuck_out_tongue_closed_eyes",
    "description": "FACE WITH STUCK-OUT TONGUE AND TIGHTLY-CLOSED EYES",
    "category": "people"
  },
  {
    "name": "stuck_out_tongue",
    "unicode": {"apple":"1F61B", "google":"1F61B", "twitter":"1F61B"},
    "shortcode": "stuck_out_tongue",
    "description": "FACE WITH STUCK-OUT TONGUE",
    "category": "people"
  },
  {
    "name": "money_mouth_face",
    "unicode": {"apple":"1F911", "google":"1F911", "twitter":"1F911"},
    "shortcode": "money_mouth_face",
    "description": "Money-Mouth Face",
    "category": "people"
  },
  {
    "name": "nerd_face",
    "unicode": {"apple":"1F913", "google":"1F913", "twitter":"1F913"},
    "shortcode": "nerd_face",
    "description": "Nerd Face",
    "category": "people"
  },
  {
    "name": "sunglasses",
    "unicode": { "apple": "1F60E", "google":"1F60E", "twitter":"1F60E"},
    "shortcode": "sunglasses",
    "description": "SMILING FACE WITH SUNGLASSES",
    "category": "people"
  },
  {
    "name": "hugging_face",
    "unicode": {"apple":"1F917", "google":"1F917", "twitter":"1F917"},
    "shortcode": "hugging_face",
    "description": "Hugging Face",
    "category": "people"
  },
  {
    "name": "smirk",
    "unicode": {"apple":"1F60F", "google":"1F60F", "twitter":"1F60F"},
    "shortcode": "smirk",
    "description": "SMIRKING FACE",
    "category": "people"
  },
  {
    "name": "no_mouth",
    "unicode": {"apple":"1F636", "google":"1F636", "twitter":"1F636"},
    "shortcode": "no_mouth",
    "description": "FACE WITHOUT MOUTH",
    "category": "people"
  },
  {
    "name": "neutral_face",
    "unicode": {"apple":"1F610", "google":"1F610", "twitter":"1F610"},
    "shortcode": "neutral_face",
    "description": "NEUTRAL FACE",
    "category": "people"
  },
  {
    "name": "expressionless",
    "unicode": {"apple":"1F611", "google":"1F611", "twitter":"1F611"},
    "shortcode": "expressionless",
    "description": "EXPRESSIONLESS FACE",
    "category": "people"
  },
  {
    "name": "unamused",
    "unicode": {"apple":"1F612", "google":"1F612", "twitter":"1F612"},
    "shortcode": "unamused",
    "description": "UNAMUSED FACE",
    "category": "people"
  },
  {
    "name": "face_with_rolling_eyes",
    "unicode": {"apple":"1F644", "google":"1F644", "twitter":"1F644"},
    "shortcode": "face_with_rolling_eyes",
    "description": "Face With Rolling Eyes",
    "category": "people"
  },
  {
    "name": "thinking_face",
    "unicode": {"apple":"1F914", "google":"1F914", "twitter":"1F914"},
    "shortcode": "thinking_face",
    "description": "Thinking Face",
    "category": "people"
  },
  {
    "name": "flushed",
    "unicode": {"apple":"1F633", "google":"1F633", "twitter":"1F633"},
    "shortcode": "flushed",
    "description": "FLUSHED FACE",
    "category": "people"
  },
  {
    "name": "disappointed",
    "unicode": {"apple":"1F61E", "google":"1F61E", "twitter":"1F61E"},
    "shortcode": "disappointed",
    "description": "DISAPPOINTED FACE",
    "category": "people"
  },
  {
    "name": "worried",
    "unicode": {"apple":"1F61F", "google":"1F61F", "twitter":"1F61F"},
    "shortcode": "worried",
    "description": "WORRIED FACE",
    "category": "people"
  },
  {
    "name": "angry",
    "unicode": {"apple":"1F620", "google":"1F620", "twitter":"1F620"},
    "shortcode": "angry",
    "description": "ANGRY FACE",
    "category": "people"
  },
  {
    "name": "rage",
    "unicode": {"apple":"1F621", "google":"1F621", "twitter":"1F621"},
    "shortcode": "rage",
    "description": "POUTING FACE",
    "category": "people"
  },
  {
    "name": "pensive",
    "unicode": {"apple":"1F614", "google":"1F614", "twitter":"1F614"},
    "shortcode": "pensive",
    "description": "PENSIVE FACE",
    "category": "people"
  },
  {
    "name": "confused",
    "unicode": {"apple":"1F615", "google":"1F615", "twitter":"1F615"},
    "shortcode": "confused",
    "description": "CONFUSED FACE",
    "category": "people"
  },
  {
    "name": "slightly_frowning_face",
    "unicode": {"apple":"1F641", "google":"1F641", "twitter":"1F641"},
    "shortcode": "slightly_frowning_face",
    "description": "slightly frowning face",
    "category": "people"
  },
  {
    "name": "white_frowning_face",
    "unicode": {"apple":"2639", "google":"2639", "twitter":"2639"},
    "shortcode": "white_frowning_face",
    "description": "white frowning face",
    "category": "people"
  },
  {
    "name": "persevere",
    "unicode": {"apple":"1F623", "google":"1F623", "twitter":"1F623"},
    "shortcode": "persevere",
    "description": "PERSEVERING FACE",
    "category": "people"
  },
  {
    "name": "confounded",
    "unicode": {"apple":"1F616", "google":"1F616", "twitter":"1F616"},
    "shortcode": "confounded",
    "description": "CONFOUNDED FACE",
    "category": "people"
  },
  {
    "name": "tired_face",
    "unicode": {"apple":"1F62B", "google":"1F62B", "twitter":"1F62B"},
    "shortcode": "tired_face",
    "description": "TIRED FACE",
    "category": "people"
  },
  {
    "name": "weary",
    "unicode": {"apple":"1F629", "google":"1F629", "twitter":"1F629"},
    "shortcode": "weary",
    "description": "WEARY FACE",
    "category": "people"
  },
  {
    "name": "triumph",
    "unicode": {"apple":"1F624", "google":"1F624", "twitter":"1F624"},
    "shortcode": "triumph",
    "description": "FACE WITH LOOK OF TRIUMPH",
    "category": "people"
  },
  {
    "name": "open_mouth",
    "unicode": {"apple":"1F62E", "google":"1F62E", "twitter":"1F62E"},
    "shortcode": "open_mouth",
    "description": "FACE WITH OPEN MOUTH",
    "category": "people"
  },
  {
    "name": "scream",
    "unicode": {"apple":"1F631", "google":"1F631", "twitter":"1F631"},
    "shortcode": "scream",
    "description": "FACE SCREAMING IN FEAR",
    "category": "people"
  },
  {
    "name": "fearful",
    "unicode": {"apple":"1F628", "google":"1F628", "twitter":"1F628"},
    "shortcode": "fearful",
    "description": "FEARFUL FACE",
    "category": "people"
  },
  {
    "name": "cold_sweat",
    "unicode": {"apple":"1F630", "google":"1F630", "twitter":"1F630"},
    "shortcode": "cold_sweat",
    "description": "FACE WITH OPEN MOUTH AND COLD SWEAT",
    "category": "people"
  },
  {
    "name": "hushed",
    "unicode": {"apple":"1F62F", "google":"1F62F", "twitter":"1F62F"},
    "shortcode": "hushed",
    "description": "HUSHED FACE",
    "category": "people"
  },
  {
    "name": "frowning",
    "unicode": {"apple":"1F626", "google":"1F626", "twitter":"1F626"},
    "shortcode": "frowning",
    "description": "FROWNING FACE WITH OPEN MOUTH",
    "category": "people"
  },
  {
    "name": "anguished",
    "unicode": {"apple":"1F627", "google":"1F627", "twitter":"1F627"},
    "shortcode": "anguished",
    "description": "ANGUISHED FACE",
    "category": "people"
  },
  {
    "name": "cry",
    "unicode": {"apple":"1F622", "google":"1F622", "twitter":"1F622"},
    "shortcode": "cry",
    "description": "CRYING FACE",
    "category": "people"
  },
  {
    "name": "disappointed_relieved",
    "unicode": {"apple":"1F625", "google":"1F625", "twitter":"1F625"},
    "shortcode": "disappointed_relieved",
    "description": "DISAPPOINTED BUT RELIEVED FACE",
    "category": "people"
  },
  {
    "name": "sleepy",
    "unicode": {"apple":"1F62A", "google":"1F62A", "twitter":"1F62A"},
    "shortcode": "sleepy",
    "description": "SLEEPY FACE",
    "category": "people"
  },
  {
    "name": "sweat",
    "unicode": {"apple":"1F613", "google":"1F613", "twitter":"1F613"},
    "shortcode": "sweat",
    "description": "FACE WITH COLD SWEAT",
    "category": "people"
  },
  {
    "name": "sob",
    "unicode": {"apple":"1F62D", "google":"1F62D", "twitter":"1F62D"},
    "shortcode": "sob",
    "description": "LOUDLY CRYING FACE",
    "category": "people"
  },
  {
    "name": "dizzy_face",
    "unicode": {"apple":"1F635", "google":"1F635", "twitter":"1F635"},
    "shortcode": "dizzy_face",
    "description": "DIZZY FACE",
    "category": "people"
  },
  {
    "name": "astonished",
    "unicode": {"apple":"1F632", "google":"1F632", "twitter":"1F632"},
    "shortcode": "astonished",
    "description": "ASTONISHED FACE",
    "category": "people"
  },
  {
    "name": "zipper_mouth_face",
    "unicode": {"apple":"1F910", "google":"1F910", "twitter":"1F910"},
    "shortcode": "zipper_mouth_face",
    "description": "Zipper-Mouth Face",
    "category": "people"
  },
  {
    "name": "mask",
    "unicode": {"apple":"1F637", "google":"1F637", "twitter":"1F637"},
    "shortcode": "mask",
    "description": "FACE WITH MEDICAL MASK",
    "category": "people"
  },
  {
    "name": "face_with_thermometer",
    "unicode": {"apple":"1F912", "google":"1F912", "twitter":"1F912"},
    "shortcode": "face_with_thermometer",
    "description": "Face With Thermometer",
    "category": "people"
  },
  {
    "name": "face_with_head_bandage",
    "unicode": {"apple":"1F915", "google":"1F915", "twitter":"1F915"},
    "shortcode": "face_with_head_bandage",
    "description": "Face With Head-Bandage",
    "category": "people"
  },
  {
    "name": "sleeping",
    "unicode": {"apple":"1F634", "google":"1F634", "twitter":"1F634"},
    "shortcode": "sleeping",
    "description": "SLEEPING FACE",
    "category": "people"
  },
  {
    "name": "zzz",
    "unicode": {"apple":"1F4A4", "google":"1F4A4", "twitter":"1F4A4"},
    "shortcode": "zzz",
    "description": "SLEEPING SYMBOL",
    "category": "people"
  },
  {
    "name": "hankey",
    "keywords": ["poop", "poo"],
    "unicode": {"apple":"1F4A9", "google":"1F4A9", "twitter":"1F4A9"},
    "shortcode": "hankey",
    "description": "PILE OF POO",
    "category": "people"
  },
  {
    "name": "smiling_imp",
    "unicode": {"apple":"1F608", "google":"1F608", "twitter":"1F608"},
    "shortcode": "smiling_imp",
    "description": "SMILING FACE WITH HORNS",
    "category": "people"
  },
  {
    "name": "imp",
    "unicode": {"apple":"1F47F", "google":"1F47F", "twitter":"1F47F"},
    "shortcode": "imp",
    "description": "IMP",
    "category": "people"
  },
  {
    "name": "japanese_ogre",
    "unicode": {"apple":"1F479", "google":"1F479", "twitter":"1F479"},
    "shortcode": "japanese_ogre",
    "description": "JAPANESE OGRE",
    "category": "people"
  },
  {
    "name": "japanese_goblin",
    "unicode": {"apple":"1F47A", "google":"1F47A", "twitter":"1F47A"},
    "shortcode": "japanese_goblin",
    "description": "JAPANESE GOBLIN",
    "category": "people"
  },
  {
    "name": "skull",
    "unicode": {"apple":"1F480", "google":"1F480", "twitter":"1F480"},
    "shortcode": "skull",
    "description": "SKULL",
    "category": "people"
  },
  {
    "name": "ghost",
    "unicode": {"apple":"1F47B", "google":"1F47B", "twitter":"1F47B"},
    "shortcode": "ghost",
    "description": "GHOST",
    "category": "people"
  },
  {
    "name": "alien",
    "unicode": {"apple":"1F47D", "google":"1F47D", "twitter":"1F47D"},
    "shortcode": "alien",
    "description": "EXTRATERRESTRIAL ALIEN",
    "category": "people"
  },
  {
    "name": "robot_face",
    "unicode": {"apple":"1F916", "google":"1F916", "twitter":"1F916"},
    "shortcode": "robot_face",
    "description": "Robot Face",
    "category": "people"
  },
  {
    "name": "smiley_cat",
    "unicode": {"apple":"1F63A", "google":"1F63A", "twitter":"1F63A"},
    "shortcode": "smiley_cat",
    "description": "SMILING CAT FACE WITH OPEN MOUTH",
    "category": "people"
  },
  {
    "name": "smile_cat",
    "unicode": {"apple":"1F638", "google":"1F638", "twitter":"1F638"},
    "shortcode": "smile_cat",
    "description": "GRINNING CAT FACE WITH SMILING EYES",
    "category": "people"
  },
  {
    "name": "joy_cat",
    "unicode": {"apple":"1F639", "google":"1F639", "twitter":"1F639"},
    "shortcode": "joy_cat",
    "description": "CAT FACE WITH TEARS OF JOY",
    "category": "people"
  },
  {
    "name": "heart_eyes_cat",
    "unicode": {"apple":"1F63B", "google":"1F63B", "twitter":"1F63B"},
    "shortcode": "heart_eyes_cat",
    "description": "SMILING CAT FACE WITH HEART-SHAPED EYES",
    "category": "people"
  },
  {
    "name": "smirk_cat",
    "unicode": {"apple":"1F63C", "google":"1F63C", "twitter":"1F63C"},
    "shortcode": "smirk_cat",
    "description": "CAT FACE WITH WRY SMILE",
    "category": "people"
  },
  {
    "name": "kissing_cat",
    "unicode": {"apple":"1F63D", "google":"1F63D", "twitter":"1F63D"},
    "shortcode": "kissing_cat",
    "description": "KISSING CAT FACE WITH CLOSED EYES",
    "category": "people"
  },
  {
    "name": "scream_cat",
    "unicode": {"apple":"1F640", "google":"1F640", "twitter":"1F640"},
    "shortcode": "scream_cat",
    "description": "WEARY CAT FACE",
    "category": "people"
  },
  {
    "name": "crying_cat_face",
    "unicode": {"apple":"1F63F", "google":"1F63F", "twitter":"1F63F"},
    "shortcode": "crying_cat_face",
    "description": "CRYING CAT FACE",
    "category": "people"
  },
  {
    "name": "pouting_cat",
    "unicode": {"apple":"1F63E", "google":"1F63E", "twitter":"1F63E"},
    "shortcode": "pouting_cat",
    "description": "POUTING CAT FACE",
    "category": "people"
  },
  {
    "name": "raised_hands",
    "unicode": {"apple":"1F64C", "google":"1F64C", "twitter":"1F64C"},
    "shortcode": "raised_hands",
    "description": "PERSON RAISING BOTH HANDS IN CELEBRATION",
    "category": "people"
  },
  {
    "name": "clap",
    "unicode": {"apple":"1F44F", "google":"1F44F", "twitter":"1F44F"},
    "shortcode": "clap",
    "description": "CLAPPING HANDS SIGN",
    "category": "people"
  },
  {
    "name": "wave",
    "unicode": {"apple":"1F44B", "google":"1F44B", "twitter":"1F44B"},
    "shortcode": "wave",
    "description": "WAVING HAND SIGN",
    "category": "people"
  },
  {
    "name": "+1",
    "keywords": ["thumbsup"],
    "unicode": {"apple":"1F44D", "google":"1F44D", "twitter":"1F44D"},
    "shortcode": "plus1",
    "description": "THUMBS UP SIGN",
    "category": "people"
  },
  {
    "name": "-1",
    "keywords": ["thumbsdown"],
    "unicode": {"apple":"1F44E", "google":"1F44E", "twitter":"1F44E"},
    "shortcode": "-1",
    "description": "THUMBS DOWN SIGN",
    "category": "people"
  },
  {
    "name": "facepunch",
    "unicode": {"apple":"1F44A", "google":"1F44A", "twitter":"1F44A"},
    "shortcode": "facepunch",
    "description": "FISTED HAND SIGN",
    "category": "people"
  },
  {
    "name": "fist",
    "unicode": {"apple":"270A", "google":"270A", "twitter":"270A"},
    "shortcode": "fist",
    "description": "RAISED FIST",
    "category": "people"
  },
  {
    "name": "v",
    "unicode": {"apple":"270C", "google":"270C", "twitter":"270C"},
    "shortcode": "v",
    "description": "VICTORY HAND",
    "category": "people"
  },
  {
    "name": "ok_hand",
    "unicode": {"apple":"1F44C", "google":"1F44C", "twitter":"1F44C"},
    "shortcode": "ok_hand",
    "description": "OK HAND SIGN",
    "category": "people"
  },
  {
    "name": "hand",
    "unicode": {"apple":"270B", "google":"270B", "twitter":"270B"},
    "shortcode": "hand",
    "description": "RAISED HAND",
    "category": "people"
  },
  {
    "name": "open_hands",
    "unicode": {"apple":"1F450", "google":"1F450", "twitter":"1F450"},
    "shortcode": "open_hands",
    "description": "OPEN HANDS SIGN",
    "category": "people"
  },
  {
    "name": "muscle",
    "unicode": {"apple":"1F4AA", "google":"1F4AA", "twitter":"1F4AA"},
    "shortcode": "muscle",
    "description": "FLEXED BICEPS",
    "category": "people"
  },
  {
    "name": "pray",
    "unicode": {"apple":"1F64F", "google":"1F64F", "twitter":"1F64F"},
    "shortcode": "pray",
    "description": "PERSON WITH FOLDED HANDS",
    "category": "people"
  },
  {
    "name": "point_up",
    "unicode": {"apple":"261D", "google":"261D", "twitter":"261D"},
    "shortcode": "point_up",
    "description": "WHITE UP POINTING INDEX",
    "category": "people"
  },
  {
    "name": "point_up_2",
    "unicode": {"apple":"1F446", "google":"1F446", "twitter":"1F446"},
    "shortcode": "point_up_2",
    "description": "WHITE UP POINTING BACKHAND INDEX",
    "category": "people"
  },
  {
    "name": "point_down",
    "unicode": {"apple":"1F447", "google":"1F447", "twitter":"1F447"},
    "shortcode": "point_down",
    "description": "WHITE DOWN POINTING BACKHAND INDEX",
    "category": "people"
  },
  {
    "name": "point_left",
    "unicode": {"apple":"1F448", "google":"1F448", "twitter":"1F448"},
    "shortcode": "point_left",
    "description": "WHITE LEFT POINTING BACKHAND INDEX",
    "category": "people"
  },
  {
    "name": "point_right",
    "unicode": {"apple":"1F449", "google":"1F449", "twitter":"1F449"},
    "shortcode": "point_right",
    "description": "WHITE RIGHT POINTING BACKHAND INDEX",
    "category": "people"
  },
  {
    "name": "middle_finger",
    "keywords": ["reversed_hand_with_middle_finger_extended"],
    "unicode": {"apple":"1F595", "google":"1F595", "twitter":"1F595"},
    "shortcode": "middle_finger",
    "description": "Reversed Hand With Middle Finger Extended",
    "category": "people"
  },
  {
    "name": "raised_hand_with_fingers_splayed",
    "unicode": {"apple":"1F590", "google":"1F590", "twitter":"1F590"},
    "shortcode": "raised_hand_with_fingers_splayed",
    "description": "Raised Hand With Fingers Splayed",
    "category": "people"
  },
  {
    "name": "the_horns",
    "keywords": ["sign_of_the_horns"],
    "unicode": {"apple":"1F918", "google":"1F918", "twitter":"1F918"},
    "shortcode": "the_horns",
    "description": "Sign of the Horns",
    "category": "people"
  },
  {
    "name": "spock-hand",
    "unicode": {"apple":"1F596", "google":"1F596", "twitter":"1F596"},
    "shortcode": "spock-hand",
    "description": "Raised Hand With Part Between Middle and Ring Fingers",
    "category": "people"
  },
  {
    "name": "writing_hand",
    "unicode": {"apple":"270D", "google":"270D", "twitter":"270D"},
    "shortcode": "writing_hand",
    "description": "Writing Hand",
    "category": "people"
  },
  {
    "name": "nail_care",
    "unicode": {"apple":"1F485", "google":"1F485", "twitter":"1F485"},
    "shortcode": "nail_care",
    "description": "NAIL POLISH",
    "category": "people"
  },
  {
    "name": "lips",
    "unicode": {"apple":"1F444", "google":"1F444", "twitter":"1F444"},
    "shortcode": "lips",
    "description": "MOUTH",
    "category": "people"
  },
  {
    "name": "tongue",
    "unicode": {"apple":"1F445", "google":"1F445", "twitter":"1F445"},
    "shortcode": "tongue",
    "description": "TONGUE",
    "category": "people"
  },
  {
    "name": "ear",
    "unicode": {"apple":"1F442", "google":"1F442", "twitter":"1F442"},
    "shortcode": "ear",
    "description": "EAR",
    "category": "people"
  },
  {
    "name": "nose",
    "unicode": {"apple":"1F443", "google":"1F443", "twitter":"1F443"},
    "shortcode": "nose",
    "description": "NOSE",
    "category": "people"
  },
  {
    "name": "eye",
    "unicode": {"apple":"1F441", "google":"1F441", "twitter":"1F441"},
    "shortcode": "eye",
    "description": "EYE",
    "category": "people"
  },
  {
    "name": "eyes",
    "unicode": { "apple": "1F440", "google":"1F440", "twitter":"1F440"},
    "shortcode": "eyes",
    "description": "EYES",
    "category": "people"
  },
  {
    "name": "bust_in_silhouette",
    "unicode": {"apple":"1F464", "google":"1F464", "twitter":"1F464"},
    "shortcode": "bust_in_silhouette",
    "description": "BUST IN SILHOUETTE",
    "category": "people"
  },
  {
    "name": "busts_in_silhouette",
    "unicode": {"apple":"1F465", "google":"1F465", "twitter":"1F465"},
    "shortcode": "busts_in_silhouette",
    "description": "BUSTS IN SILHOUETTE",
    "category": "people"
  },
  {
    "name": "speaking_head_in_silhouette",
    "unicode": {"apple":"1F5E3", "google":"1F5E3", "twitter":"1F5E3"},
    "shortcode": "speaking_head_in_silhouette",
    "description": " Speaking Head in Silhouette",
    "category": "people"
  },
  {
    "name": "baby",
    "unicode": {"apple":"1F476", "google":"1F476", "twitter":"1F476"},
    "shortcode": "baby",
    "description": "BABY",
    "category": "people"
  },
  {
    "name": "boy",
    "unicode": {"apple":"1F466", "google":"1F466", "twitter":"1F466"},
    "shortcode": "boy",
    "description": "BOY",
    "category": "people"
  },
  {
    "name": "girl",
    "unicode": {"apple":"1F467", "google":"1F467", "twitter":"1F467"},
    "shortcode": "girl",
    "description": "GIRL",
    "category": "people"
  },
  {
    "name": "man",
    "unicode": {"apple":"1F468", "google":"1F468", "twitter":"1F468"},
    "shortcode": "man",
    "description": "MAN",
    "category": "people"
  },
  {
    "name": "woman",
    "unicode": {"apple":"1F469", "google":"1F469", "twitter":"1F469"},
    "shortcode": "woman",
    "description": "WOMAN",
    "category": "people"
  },
  {
    "name": "person_with_blond_hair",
    "unicode": {"apple":"1F471", "google":"1F471", "twitter":"1F471"},
    "shortcode": "person_with_blond_hair",
    "description": "PERSON WITH BLOND HAIR",
    "category": "people"
  },
  {
    "name": "older_man",
    "unicode": {"apple":"1F474", "google":"1F474", "twitter":"1F474"},
    "shortcode": "older_man",
    "description": "OLDER MAN",
    "category": "people"
  },
  {
    "name": "older_woman",
    "unicode": {"apple":"1F475", "google":"1F475", "twitter":"1F475"},
    "shortcode": "older_woman",
    "description": "OLDER WOMAN",
    "category": "people"
  },
  {
    "name": "man_with_gua_pi_mao",
    "unicode": {"apple":"1F472", "google":"1F472", "twitter":"1F472"},
    "shortcode": "man_with_gua_pi_mao",
    "description": "MAN WITH GUA PI MAO",
    "category": "people"
  },
  {
    "name": "man_with_turban",
    "unicode": {"apple":"1F473", "google":"1F473", "twitter":"1F473"},
    "shortcode": "man_with_turban",
    "description": "MAN WITH TURBAN",
    "category": "people"
  },
  {
    "name": "cop",
    "unicode": {"apple":"1F46E", "google":"1F46E", "twitter":"1F46E"},
    "shortcode": "cop",
    "description": "POLICE OFFICER",
    "category": "people"
  },
  {
    "name": "construction_worker",
    "unicode": {"apple":"1F477", "google":"1F477", "twitter":"1F477"},
    "shortcode": "construction_worker",
    "description": "CONSTRUCTION WORKER",
    "category": "people"
  },
  {
    "name": "guardsman",
    "unicode": {"apple":"1F482", "google":"1F482", "twitter":"1F482"},
    "shortcode": "guardsman",
    "description": "GUARDSMAN",
    "category": "people"
  },
  {
    "name": "sleuth_or_spy",
    "unicode": {"apple":"1F575", "google":"1F575", "twitter":"1F575"},
    "shortcode": "sleuth_or_spy",
    "description": "Sleuth Or Spy",
    "category": "people"
  },
  {
    "name": "santa",
    "unicode": {"apple":"1F385", "google":"1F385", "twitter":"1F385"},
    "shortcode": "santa",
    "description": "FATHER CHRISTMAS",
    "category": "people"
  },
  {
    "name": "angel",
    "unicode": {"apple":"1F47C", "google":"1F47C", "twitter":"1F47C"},
    "shortcode": "angel",
    "description": "BABY ANGEL",
    "category": "people"
  },
  {
    "name": "princess",
    "unicode": {"apple":"1F478", "google":"1F478", "twitter":"1F478"},
    "shortcode": "princess",
    "description": "PRINCESS",
    "category": "people"
  },
  {
    "name": "bride_with_veil",
    "unicode": {"apple":"1F470", "google":"1F470", "twitter":"1F470"},
    "shortcode": "bride_with_veil",
    "description": "BRIDE WITH VEIL",
    "category": "people"
  },
  {
    "name": "walking",
    "unicode": {"apple":"1F6B6", "google":"1F6B6", "twitter":"1F6B6"},
    "shortcode": "walking",
    "description": "PEDESTRIAN",
    "category": "people"
  },
  {
    "name": "runner",
    "unicode": {"apple":"1F3C3", "google":"1F3C3", "twitter":"1F3C3"},
    "shortcode": "runner",
    "description": "RUNNER",
    "category": "people"
  },
  {
    "name": "dancer",
    "unicode": {"apple":"1F483", "google":"1F483", "twitter":"1F483"},
    "shortcode": "dancer",
    "description": "DANCER",
    "category": "people"
  },
  {
    "name": "dancers",
    "unicode": {"apple":"1F46F", "google":"1F46F", "twitter":"1F46F"},
    "shortcode": "dancers",
    "description": "WOMAN WITH BUNNY EARS",
    "category": "people"
  },
  {
    "name": "couple",
    "unicode": {"apple":"1F46B", "google":"1F46B", "twitter":"1F46B"},
    "shortcode": "couple",
    "description": "MAN AND WOMAN HOLDING HANDS",
    "category": "people"
  },
  {
    "name": "two_men_holding_hands",
    "unicode": {"apple":"1F46C", "google":"1F46C", "twitter":"1F46C"},
    "shortcode": "two_men_holding_hands",
    "description": "TWO MEN HOLDING HANDS",
    "category": "people"
  },
  {
    "name": "two_women_holding_hands",
    "unicode": {"apple":"1F46D", "google":"1F46D", "twitter":"1F46D"},
    "shortcode": "two_women_holding_hands",
    "description": "TWO WOMEN HOLDING HANDS",
    "category": "people"
  },
  {
    "name": "bow",
    "unicode": {"apple":"1F647", "google":"1F647", "twitter":"1F647"},
    "shortcode": "bow",
    "description": "PERSON BOWING DEEPLY",
    "category": "people"
  },
  {
    "name": "information_desk_person",
    "unicode": {"apple":"1F481", "google":"1F481", "twitter":"1F481"},
    "shortcode": "information_desk_person",
    "description": "INFORMATION DESK PERSON",
    "category": "people"
  },
  {
    "name": "no_good",
    "unicode": {"apple":"1F645", "google":"1F645", "twitter":"1F645"},
    "shortcode": "no_good",
    "description": "FACE WITH NO GOOD GESTURE",
    "category": "people"
  },
  {
    "name": "ok_woman",
    "unicode": {"apple":"1F646", "google":"1F646", "twitter":"1F646"},
    "shortcode": "ok_woman",
    "description": "FACE WITH OK GESTURE",
    "category": "people"
  },
  {
    "name": "raising_hand",
    "unicode": {"apple":"1F64B", "google":"1F64B", "twitter":"1F64B"},
    "shortcode": "raising_hand",
    "description": "HAPPY PERSON RAISING ONE HAND",
    "category": "people"
  },
  {
    "name": "person_with_pouting_face",
    "unicode": {"apple":"1F64E", "google":"1F64E", "twitter":"1F64E"},
    "shortcode": "person_with_pouting_face",
    "description": "PERSON WITH POUTING FACE",
    "category": "people"
  },
  {
    "name": "person_frowning",
    "unicode": {"apple":"1F64D", "google":"1F64D", "twitter":"1F64D"},
    "shortcode": "person_frowning",
    "description": "PERSON FROWNING",
    "category": "people"
  },
  {
    "name": "haircut",
    "unicode": {"apple":"1F487", "google":"1F487", "twitter":"1F487"},
    "shortcode": "haircut",
    "description": "HAIRCUT",
    "category": "people"
  },
  {
    "name": "massage",
    "unicode": {"apple":"1F486", "google":"1F486", "twitter":"1F486"},
    "shortcode": "massage",
    "description": "FACE MASSAGE",
    "category": "people"
  },
  {
    "name": "couple_with_heart",
    "unicode": {"apple":"1F491", "google":"1F491", "twitter":"1F491"},
    "shortcode": "couple_with_heart",
    "description": "COUPLE WITH HEART",
    "category": "people"
  },
  // {
  //   "name": "woman-heart-woman",
  //   "unicode": {"apple":"1F469-200D-2764-FE0F-200D-1F469", "google":"1F469-200D-2764-FE0F-200D-1F469", "twitter":"1F469-200D-2764-FE0F-200D-1F469"},
  //   "shortcode": "woman-heart-woman",
  //   "description": "Couple With Heart (Woman, Woman)",
  //   "category": "people"
  // },
  // {
  //   "name": "man-heart-man",
  //   "unicode": {"apple":"1F468-200D-2764-FE0F-200D-1F468", "google":"1F468-200D-2764-FE0F-200D-1F468", "twitter":"1F468-200D-2764-FE0F-200D-1F468"},
  //   "shortcode": "man-heart-man",
  //   "description": "COUPLE WITH HEART",
  //   "category": "people"
  // },
  {
    "name": "couplekiss",
    "unicode": {"apple":"1F48F", "google":"1F48F", "twitter":"1F48F"},
    "shortcode": "couplekiss",
    "description": "KISS",
    "category": "people"
  },
  // {
  //   "name": "woman-kiss-woman",
  //   "unicode": {"apple":"1F469-200D-2764-FE0F-200D-1F48B-200D-1F469", "google":"1F469-200D-2764-FE0F-200D-1F48B-200D-1F469", "twitter":"1F469-200D-2764-FE0F-200D-1F48B-200D-1F469"},
  //   "shortcode": "woman-kiss-woman",
  //   "description": "KISS",
  //   "category": "people"
  // },
  // {
  //   "name": "man-kiss-man",
  //   "unicode": {"apple":"1F468-200D-2764-FE0F-200D-1F48B-200D-1F468", "google":"1F468-200D-2764-FE0F-200D-1F48B-200D-1F468", "twitter":"1F468-200D-2764-FE0F-200D-1F48B-200D-1F468"},
  //   "shortcode": "man-kiss-man",
  //   "description": "KISS",
  //   "category": "people"
  // },
  {
    "name": "family",
    "unicode": {"apple":"1F46A", "google":"1F46A", "twitter":"1F46A"},
    "shortcode": "family",
    "description": "FAMILY",
    "category": "people"
  },
  // {
  //   "name": "man-woman-girl",
  //   "unicode": {"apple":"1F468-200D-1F469-200D-1F467", "google":"1F468-200D-1F469-200D-1F467", "twitter":"1F468-200D-1F469-200D-1F467"},
  //   "shortcode": "man-woman-girl",
  //   "description": "FAMILY",
  //   "category": "people"
  // },
  // {
  //   "name": "man-woman-girl-boy",
  //   "unicode": {"apple":"1F469-200D-1F469-200D-1F467-200D-1F466", "google":"1F469-200D-1F469-200D-1F467-200D-1F466", "twitter":"1F469-200D-1F469-200D-1F467-200D-1F466"},
  //   "shortcode": "man-woman-girl-boy",
  //   "description": "FAMILY",
  //   "category": "people"
  // },
  // {
  //   "name": "man-woman-boy-boy",
  //   "unicode": {"apple":"1F468-200D-1F469-200D-1F466-200D-1F466", "google":"1F468-200D-1F469-200D-1F466-200D-1F466", "twitter":"1F468-200D-1F469-200D-1F466-200D-1F466"},
  //   "shortcode": "man-woman-boy-boy",
  //   "description": "FAMILY",
  //   "category": "people"
  // },
  // {
  //   "name": "man-woman-girl-girl",
  //   "unicode": {"apple":"1F468-200D-1F469-200D-1F467-200D-1F467", "google":"1F468-200D-1F469-200D-1F467-200D-1F467", "twitter":"1F468-200D-1F469-200D-1F467-200D-1F467"},
  //   "shortcode": "man-woman-girl-girl",
  //   "description": "FAMILY",
  //   "category": "people"
  // },
  // {
  //   "name": "woman-woman-boy",
  //   "unicode": {"apple":"1F469-200D-1F469-200D-1F466", "google":"1F469-200D-1F469-200D-1F466", "twitter":"1F469-200D-1F469-200D-1F466"},
  //   "shortcode": "woman-woman-boy",
  //   "description": "FAMILY",
  //   "category": "people"
  // },
  // {
  //   "name": "woman-woman-girl",
  //   "unicode": {"apple":"1F469-200D-1F469-200D-1F467", "google":"1F469-200D-1F469-200D-1F467", "twitter":"1F469-200D-1F469-200D-1F467"},
  //   "shortcode": "woman-woman-girl",
  //   "description": "FAMILY",
  //   "category": "people"
  // },
  // {
  //   "name": "woman-woman-girl-boy",
  //   "unicode": {"apple":"1F469-200D-1F469-200D-1F467-200D-1F466", "google":"1F469-200D-1F469-200D-1F467-200D-1F466", "twitter":"1F469-200D-1F469-200D-1F467-200D-1F466"},
  //   "shortcode": "woman-woman-girl-boy",
  //   "description": "FAMILY",
  //   "category": "people"
  // },
  // {
  //   "name": "woman-woman-boy-boy",
  //   "unicode": {"apple":"1F469-200D-1F469-200D-1F466-200D-1F466", "google":"1F469-200D-1F469-200D-1F466-200D-1F466", "twitter":"1F469-200D-1F469-200D-1F466-200D-1F466"},
  //   "shortcode": "woman-woman-boy-boy",
  //   "description": "FAMILY",
  //   "category": "people"
  // },
  // {
  //   "name": "woman-woman-girl-girl",
  //   "unicode": {"apple":"1F469-200D-1F469-200D-1F467-200D-1F467", "google":"1F469-200D-1F469-200D-1F467-200D-1F467", "twitter":"1F469-200D-1F469-200D-1F467-200D-1F467"},
  //   "shortcode": "woman-woman-girl-girl",
  //   "description": "FAMILY",
  //   "category": "people"
  // },
  // {
  //   "name": "man-man-boy",
  //   "unicode": {"apple":"1F468-200D-1F468-200D-1F466", "google":"1F468-200D-1F468-200D-1F466", "twitter":"1F468-200D-1F468-200D-1F466"},
  //   "shortcode": "man-man-boy",
  //   "description": "FAMILY",
  //   "category": "people"
  // },
  // {
  //   "name": "man-man-girl",
  //   "unicode": {"apple":"1F468-200D-1F468-200D-1F467", "google":"1F468-200D-1F468-200D-1F467", "twitter":"1F468-200D-1F468-200D-1F467"},
  //   "shortcode": "man-man-girl",
  //   "description": "FAMILY",
  //   "category": "people"
  // },
  // {
  //   "name": "man-man-girl-boy",
  //   "unicode": {"apple":"1F468-200D-1F468-200D-1F467-200D-1F466", "google":"1F468-200D-1F468-200D-1F467-200D-1F466", "twitter":"1F468-200D-1F468-200D-1F467-200D-1F466"},
  //   "shortcode": "man-man-girl-boy",
  //   "description": "FAMILY",
  //   "category": "people"
  // },
  // {
  //   "name": "man-man-boy-boy",
  //   "unicode": {"apple":"1F468-200D-1F468-200D-1F466-200D-1F466", "google":"1F468-200D-1F468-200D-1F466-200D-1F466", "twitter":"1F468-200D-1F468-200D-1F466-200D-1F466"},
  //   "shortcode": "man-man-boy-boy",
  //   "description": "FAMILY",
  //   "category": "people"
  // },
  // {
  //   "name": "man-man-girl-girl",
  //   "unicode": {"apple":"1F468-200D-1F468-200D-1F467-200D-1F467", "google":"1F468-200D-1F468-200D-1F467-200D-1F467", "twitter":"1F468-200D-1F468-200D-1F467-200D-1F467"},
  //   "shortcode": "man-man-girl-girl",
  //   "description": "FAMILY",
  //   "category": "people"
  // },
  {
    "name": "womans_clothes",
    "unicode": {"apple":"1F45A", "google":"1F45A", "twitter":"1F45A"},
    "shortcode": "womans_clothes",
    "description": "WOMANS CLOTHES",
    "category": "people"
  },
  {
    "name": "shirt",
    "unicode": {"apple":"1F455", "google":"1F455", "twitter":"1F455"},
    "shortcode": "shirt",
    "description": "T-SHIRT",
    "category": "people"
  },
  {
    "name": "jeans",
    "unicode": {"apple":"1F456", "google":"1F456", "twitter":"1F456"},
    "shortcode": "jeans",
    "description": "JEANS",
    "category": "people"
  },
  {
    "name": "necktie",
    "unicode": {"apple":"1F454", "google":"1F454", "twitter":"1F454"},
    "shortcode": "necktie",
    "description": "NECKTIE",
    "category": "people"
  },
  {
    "name": "dress",
    "unicode": {"apple":"1F457", "google":"1F457", "twitter":"1F457"},
    "shortcode": "dress",
    "description": "DRESS",
    "category": "people"
  },
  {
    "name": "bikini",
    "unicode": {"apple":"1F459", "google":"1F459", "twitter":"1F459"},
    "shortcode": "bikini",
    "description": "BIKINI",
    "category": "people"
  },
  {
    "name": "kimono",
    "unicode": {"apple":"1F458", "google":"1F458", "twitter":"1F458"},
    "shortcode": "kimono",
    "description": "KIMONO",
    "category": "people"
  },
  {
    "name": "lipstick",
    "unicode": {"apple":"1F484", "google":"1F484", "twitter":"1F484"},
    "shortcode": "lipstick",
    "description": "LIPSTICK",
    "category": "people"
  },
  {
    "name": "kiss",
    "unicode": {"apple":"1F48B", "google":"1F48B", "twitter":"1F48B"},
    "shortcode": "kiss",
    "description": "KISS MARK",
    "category": "people"
  },
  {
    "name": "footprints",
    "unicode": {"apple":"1F463", "google":"1F463", "twitter":"1F463"},
    "shortcode": "footprints",
    "description": "FOOTPRINTS",
    "category": "people"
  },
  {
    "name": "high_heel",
    "unicode": {"apple":"1F460", "google":"1F460", "twitter":"1F460"},
    "shortcode": "high_heel",
    "description": "HIGH-HEELED SHOE",
    "category": "people"
  },
  {
    "name": "sandal",
    "unicode": {"apple":"1F461", "google":"1F461", "twitter":"1F461"},
    "shortcode": "sandal",
    "description": "WOMANS SANDAL",
    "category": "people"
  },
  {
    "name": "boot",
    "unicode": {"apple":"1F462", "google":"1F462", "twitter":"1F462"},
    "shortcode": "boot",
    "description": "WOMANS BOOTS",
    "category": "people"
  },
  {
    "name": "mans_shoe",
    "unicode": {"apple":"1F45E", "google":"1F45E", "twitter":"1F45E"},
    "shortcode": "mans_shoe",
    "description": "MANS SHOE",
    "category": "people"
  },
  {
    "name": "athletic_shoe",
    "unicode": {"apple":"1F45F", "google":"1F45F", "twitter":"1F45F"},
    "shortcode": "athletic_shoe",
    "description": "ATHLETIC SHOE",
    "category": "people"
  },
  {
    "name": "womans_hat",
    "unicode": {"apple":"1F452", "google":"1F452", "twitter":"1F452"},
    "shortcode": "womans_hat",
    "description": "WOMANS HAT",
    "category": "people"
  },
  {
    "name": "tophat",
    "unicode": {"apple":"1F3A9", "google":"1F3A9", "twitter":"1F3A9"},
    "shortcode": "tophat",
    "description": "TOP HAT",
    "category": "people"
  },
  {
    "name": "helmet_with_white_cross",
    "unicode": {"apple":"26D1", "google":"26D1", "twitter":"26D1"},
    "shortcode": "helmet_with_white_cross",
    "description": "Helmet With White Cross",
    "category": "people"
  },
  {
    "name": "mortar_board",
    "unicode": {"apple":"1F393", "google":"1F393", "twitter":"1F393"},
    "shortcode": "mortar_board",
    "description": "GRADUATION CAP",
    "category": "people"
  },
  {
    "name": "crown",
    "unicode": {"apple":"1F451", "google":"1F451", "twitter":"1F451"},
    "shortcode": "crown",
    "description": "CROWN",
    "category": "people"
  },
  {
    "name": "school_satchel",
    "unicode": {"apple":"1F392", "google":"1F392", "twitter":"1F392"},
    "shortcode": "school_satchel",
    "description": "SCHOOL SATCHEL",
    "category": "people"
  },
  {
    "name": "pouch",
    "unicode": {"apple":"1F45D", "google":"1F45D", "twitter":"1F45D"},
    "shortcode": "pouch",
    "description": "POUCH",
    "category": "people"
  },
  {
    "name": "purse",
    "unicode": {"apple":"1F45B", "google":"1F45B", "twitter":"1F45B"},
    "shortcode": "purse",
    "description": "PURSE",
    "category": "people"
  },
  {
    "name": "handbag",
    "unicode": {"apple":"1F45C", "google":"1F45C", "twitter":"1F45C"},
    "shortcode": "handbag",
    "description": "HANDBAG",
    "category": "people"
  },
  {
    "name": "briefcase",
    "unicode": {"apple":"1F4BC", "google":"1F4BC", "twitter":"1F4BC"},
    "shortcode": "briefcase",
    "description": "BRIEFCASE",
    "category": "people"
  },
  {
    "name": "eyeglasses",
    "unicode": {"apple":"1F453", "google":"1F453", "twitter":"1F453"},
    "shortcode": "eyeglasses",
    "description": "EYEGLASSES",
    "category": "people"
  },
  {
    "name": "dark_sunglasses",
    "unicode": {"apple":"1F576", "google":"1F576", "twitter":"1F576"},
    "shortcode": "dark_sunglasses",
    "description": "Dark Sunglasses",
    "category": "people"
  },
  {
    "name": "ring",
    "unicode": {"apple":"1F48D", "google":"1F48D", "twitter":"1F48D"},
    "shortcode": "ring",
    "description": "RING",
    "category": "people"
  },
  {
    "name": "closed_umbrella",
    "unicode": {"apple":"1F302", "google":"1F302", "twitter":"1F302"},
    "shortcode": "closed_umbrella",
    "description": "CLOSED UMBRELLA",
    "category": "people"
  },
  {
    "name": "dog",
    "unicode": {"apple":"1F436", "google":"1F436", "twitter":"1F436"},
    "shortcode": "dog",
    "description": "DOG FACE",
    "category": "animal"
  },
  {
    "name": "cat",
    "unicode": {"apple":"1F431", "google":"1F431", "twitter":"1F431"},
    "shortcode": "cat",
    "description": "CAT FACE",
    "category": "animal"
  },
  {
    "name": "mouse",
    "unicode": {"apple":"1F42D", "google":"1F42D", "twitter":"1F42D"},
    "shortcode": "mouse",
    "description": "MOUSE FACE",
    "category": "animal"
  },
  {
    "name": "hamster",
    "unicode": {"apple":"1F439", "google":"1F439", "twitter":"1F439"},
    "shortcode": "hamster",
    "description": "HAMSTER FACE",
    "category": "animal"
  },
  {
    "name": "rabbit",
    "unicode": {"apple":"1F430", "google":"1F430", "twitter":"1F430"},
    "shortcode": "rabbit",
    "description": "RABBIT FACE",
    "category": "animal"
  },
  {
    "name": "bear",
    "unicode": {"apple":"1F43B", "google":"1F43B", "twitter":"1F43B"},
    "shortcode": "bear",
    "description": "BEAR FACE",
    "category": "animal"
  },
  {
    "name": "panda_face",
    "unicode": {"apple":"1F43C", "google":"1F43C", "twitter":"1F43C"},
    "shortcode": "panda_face",
    "description": "PANDA FACE",
    "category": "animal"
  },
  {
    "name": "koala",
    "unicode": {"apple":"1F428", "google":"1F428", "twitter":"1F428"},
    "shortcode": "koala",
    "description": "KOALA",
    "category": "animal"
  },
  {
    "name": "tiger",
    "unicode": {"apple":"1F42F", "google":"1F42F", "twitter":"1F42F"},
    "shortcode": "tiger",
    "description": "TIGER FACE",
    "category": "animal"
  },
  {
    "name": "lion",
    "keywords": ["lion_face"],
    "unicode": {"apple":"1F981", "google":"1F981", "twitter":"1F981"},
    "shortcode": "lion",
    "description": "Lion Face",
    "category": "animal"
  },
  {
    "name": "cow",
    "unicode": {"apple":"1F42E", "google":"1F42E", "twitter":"1F42E"},
    "shortcode": "cow",
    "description": "COW FACE",
    "category": "animal"
  },
  {
    "name": "pig",
    "unicode": {"apple":"1F437", "google":"1F437", "twitter":"1F437"},
    "shortcode": "pig",
    "description": "PIG FACE",
    "category": "animal"
  },
  {
    "name": "pig_nose",
    "unicode": {"apple":"1F43D", "google":"1F43D", "twitter":"1F43D"},
    "shortcode": "pig_nose",
    "description": "PIG NOSE",
    "category": "animal"
  },
  {
    "name": "frog",
    "unicode": {"apple":"1F438", "google":"1F438", "twitter":"1F438"},
    "shortcode": "frog",
    "description": "FROG FACE",
    "category": "animal"
  },
  {
    "name": "octopus",
    "unicode": {"apple":"1F419", "google":"1F419", "twitter":"1F419"},
    "shortcode": "octopus",
    "description": "OCTOPUS",
    "category": "animal"
  },
  {
    "name": "monkey_face",
    "unicode": {"apple":"1F435", "google":"1F435", "twitter":"1F435"},
    "shortcode": "monkey_face",
    "description": "MONKEY FACE",
    "category": "animal"
  },
  {
    "name": "see_no_evil",
    "unicode": {"apple":"1F648", "google":"1F648", "twitter":"1F648"},
    "shortcode": "see_no_evil",
    "description": "SEE-NO-EVIL MONKEY",
    "category": "nature"
  },
  {
    "name": "hear_no_evil",
    "unicode": {"apple":"1F649", "google":"1F649", "twitter":"1F649"},
    "shortcode": "hear_no_evil",
    "description": "HEAR-NO-EVIL MONKEY",
    "category": "nature"
  },
  {
    "name": "speak_no_evil",
    "unicode": {"apple":"1F64A", "google":"1F64A", "twitter":"1F64A"},
    "shortcode": "speak_no_evil",
    "description": "SPEAK-NO-EVIL MONKEY",
    "category": "nature"
  },
  {
    "name": "monkey",
    "unicode": {"apple":"1F412", "google":"1F412", "twitter":"1F412"},
    "shortcode": "monkey",
    "description": "MONKEY",
    "category": "animal"
  },
  {
    "name": "chicken",
    "unicode": {"apple":"1F414", "google":"1F414", "twitter":"1F414"},
    "shortcode": "chicken",
    "description": "CHICKEN",
    "category": "animal"
  },
  {
    "name": "penguin",
    "unicode": {"apple":"1F427", "google":"1F427", "twitter":"1F427"},
    "shortcode": "penguin",
    "description": "PENGUIN",
    "category": "animal"
  },
  {
    "name": "bird",
    "unicode": {"apple":"1F426", "google":"1F426", "twitter":"1F426"},
    "shortcode": "bird",
    "description": "BIRD",
    "category": "animal"
  },
  {
    "name": "baby_chick",
    "unicode": {"apple":"1F424", "google":"1F424", "twitter":"1F424"},
    "shortcode": "baby_chick",
    "description": "BABY CHICK",
    "category": "animal"
  },
  {
    "name": "hatching_chick",
    "unicode": {"apple":"1F423", "google":"1F423", "twitter":"1F423"},
    "shortcode": "hatching_chick",
    "description": "HATCHING CHICK",
    "category": "animal"
  },
  {
    "name": "hatched_chick",
    "unicode": {"apple":"1F425", "google":"1F425", "twitter":"1F425"},
    "shortcode": "hatched_chick",
    "description": "FRONT-FACING BABY CHICK",
    "category": "animal"
  },
  {
    "name": "wolf",
    "unicode": {"apple":"1F43A", "google":"1F43A", "twitter":"1F43A"},
    "shortcode": "wolf",
    "description": "WOLF FACE",
    "category": "animal"
  },
  {
    "name": "boar",
    "unicode": {"apple":"1F417", "google":"1F417", "twitter":"1F417"},
    "shortcode": "boar",
    "description": "BOAR",
    "category": "animal"
  },
  {
    "name": "horse",
    "unicode": {"apple":"1F434", "google":"1F434", "twitter":"1F434"},
    "shortcode": "horse",
    "description": "HORSE FACE",
    "category": "animal"
  },
  {
    "name": "unicorn",
    "unicode": {"apple":"1F984", "google":"1F984", "twitter":"1F984"},
    "shortcode": "unicorn",
    "description": "Unicorn Face",
    "category": "animal"
  },
  {
    "name": "bee",
    "unicode": {"apple":"1F41D", "google":"1F41D", "twitter":"1F41D"},
    "shortcode": "bee",
    "description": "HONEYBEE",
    "category": "animal"
  },
  {
    "name": "bug",
    "unicode": {"apple":"1F41B", "google":"1F41B", "twitter":"1F41B"},
    "shortcode": "bug",
    "description": "BUG",
    "category": "animal"
  },
  {
    "name": "snail",
    "unicode": {"apple":"1F40C", "google":"1F40C", "twitter":"1F40C"},
    "shortcode": "snail",
    "description": "SNAIL",
    "category": "animal"
  },
  {
    "name": "beetle",
    "unicode": {"apple":"1F41E", "google":"1F41E", "twitter":"1F41E"},
    "shortcode": "beetle",
    "description": "LADY BEETLE",
    "category": "animal"
  },
  {
    "name": "ant",
    "unicode": {"apple":"1F41C", "google":"1F41C", "twitter":"1F41C"},
    "shortcode": "ant",
    "description": "ANT",
    "category": "animal"
  },
  {
    "name": "spider",
    "unicode": {"apple":"1F577", "google":"1F577", "twitter":"1F577"},
    "shortcode": "spider",
    "description": "Spider",
    "category": "animal"
  },
  {
    "name": "scorpion",
    "unicode": {"apple":"1F982", "google":"1F982", "twitter":"1F982"},
    "shortcode": "scorpion",
    "description": "Scorpion",
    "category": "animal"
  },
  {
    "name": "crab",
    "unicode": {"apple":"1F980", "google":"1F980", "twitter":"1F980"},
    "shortcode": "crab",
    "description": "Crab",
    "category": "animal"
  },
  {
    "name": "snake",
    "unicode": {"apple":"1F40D", "google":"1F40D", "twitter":"1F40D"},
    "shortcode": "snake",
    "description": "SNAKE",
    "category": "animal"
  },
  {
    "name": "turtle",
    "unicode": {"apple":"1F422", "google":"1F422", "twitter":"1F422"},
    "shortcode": "turtle",
    "description": "TURTLE",
    "category": "animal"
  },
  {
    "name": "tropical_fish",
    "unicode": {"apple":"1F420", "google":"1F420", "twitter":"1F420"},
    "shortcode": "tropical_fish",
    "description": "TROPICAL FISH",
    "category": "animal"
  },
  {
    "name": "fish",
    "unicode": {"apple":"1F41F", "google":"1F41F", "twitter":"1F41F"},
    "shortcode": "fish",
    "description": "FISH",
    "category": "animal"
  },

  {
    "name": "blowfish",
    "unicode": {"apple":"1F421", "google":"1F421", "twitter":"1F421"},
    "shortcode": "blowfish",
    "description": "BLOWFISH",
    "category": "animal"
  },
  {
    "name": "dolphin",
    "unicode": {"apple":"1F42C", "google":"1F42C", "twitter":"1F42C"},
    "shortcode": "dolphin",
    "description": "DOLPHIN",
    "category": "animal"
  },
  {
    "name": "whale",
    "unicode": {"apple":"1F433", "google":"1F433", "twitter":"1F433"},
    "shortcode": "whale",
    "description": "SPOUTING WHALE",
    "category": "animal"
  },
  {
    "name": "whale2",
    "unicode": {"apple":"1F40B", "google":"1F40B", "twitter":"1F40B"},
    "shortcode": "whale2",
    "description": "WHALE",
    "category": "animal"
  },
  {
    "name": "crocodile",
    "unicode": {"apple":"1F40A", "google":"1F40A", "twitter":"1F40A"},
    "shortcode": "crocodile",
    "description": "CROCODILE",
    "category": "animal"
  },
  {
    "name": "leopard",
    "unicode": {"apple":"1F406", "google":"1F406", "twitter":"1F406"},
    "shortcode": "leopard",
    "description": "LEOPARD",
    "category": "animal"
  },
  {
    "name": "tiger2",
    "unicode": {"apple":"1F405", "google":"1F405", "twitter":"1F405"},
    "shortcode": "tiger2",
    "description": "TIGER",
    "category": "animal"
  },
  {
    "name": "water_buffalo",
    "unicode": {"apple":"1F403", "google":"1F403", "twitter":"1F403"},
    "shortcode": "water_buffalo",
    "description": "WATER BUFFALO",
    "category": "animal"
  },
  {
    "name": "ox",
    "unicode": {"apple":"1F402", "google":"1F402", "twitter":"1F402"},
    "shortcode": "ox",
    "description": "OX",
    "category": "animal"
  },
  {
    "name": "cow2",
    "unicode": {"apple":"1F404", "google":"1F404", "twitter":"1F404"},
    "shortcode": "cow2",
    "description": "COW",
    "category": "animal"
  },
  {
    "name": "dromedary_camel",
    "unicode": {"apple":"1F42A", "google":"1F42A", "twitter":"1F42A"},
    "shortcode": "dromedary_camel",
    "description": "DROMEDARY CAMEL",
    "category": "animal"
  },
  {
    "name": "camel",
    "unicode": {"apple":"1F42B", "google":"1F42B", "twitter":"1F42B"},
    "shortcode": "camel",
    "description": "BACTRIAN CAMEL",
    "category": "animal"
  },
  {
    "name": "elephant",
    "unicode": {"apple":"1F418", "google":"1F418", "twitter":"1F418"},
    "shortcode": "elephant",
    "description": "ELEPHANT",
    "category": "animal"
  },
  {
    "name": "goat",
    "unicode": {"apple":"1F410", "google":"1F410", "twitter":"1F410"},
    "shortcode": "goat",
    "description": "GOAT",
    "category": "animal"
  },
  {
    "name": "ram",
    "unicode": {"apple":"1F40F", "google":"1F40F", "twitter":"1F40F"},
    "shortcode": "ram",
    "description": "RAM",
    "category": "animal"
  },
  {
    "name": "sheep",
    "unicode": {"apple":"1F411", "google":"1F411", "twitter":"1F411"},
    "shortcode": "sheep",
    "description": "SHEEP",
    "category": "animal"
  },
  {
    "name": "racehorse",
    "unicode": {"apple":"1F40E", "google":"1F40E", "twitter":"1F40E"},
    "shortcode": "racehorse",
    "description": "HORSE",
    "category": "animal"
  },
  {
    "name": "pig2",
    "unicode": {"apple":"1F416", "google":"1F416", "twitter":"1F416"},
    "shortcode": "pig2",
    "description": "PIG",
    "category": "animal"
  },
  {
    "name": "rat",
    "unicode": {"apple":"1F400", "google":"1F400", "twitter":"1F400"},
    "shortcode": "rat",
    "description": "RAT",
    "category": "animal"
  },
  {
    "name": "mouse2",
    "unicode": {"apple":"1F401", "google":"1F401", "twitter":"1F401"},
    "shortcode": "mouse2",
    "description": "MOUSE",
    "category": "animal"
  },
  {
    "name": "rooster",
    "unicode": {"apple":"1F413", "google":"1F413", "twitter":"1F413"},
    "shortcode": "rooster",
    "description": "ROOSTER",
    "category": "animal"
  },
  {
    "name": "turkey",
    "unicode": {"apple":"1F983", "google":"1F983", "twitter":"1F983"},
    "shortcode": "turkey",
    "description": "Turkey",
    "category": "animal"
  },
  {
    "name": "dove_of_peace",
    "unicode": {"apple":"1F54A", "google":"1F54A", "twitter":"1F54A"},
    "shortcode": "dove_of_peace",
    "description": "Dove Of Peace",
    "category": "animal"
  },
  {
    "name": "dog2",
    "unicode": {"apple":"1F415", "google":"1F415", "twitter":"1F415"},
    "shortcode": "dog2",
    "description": "DOG",
    "category": "animal"
  },
  {
    "name": "poodle",
    "unicode": {"apple":"1F429", "google":"1F429", "twitter":"1F429"},
    "shortcode": "poodle",
    "description": "POODLE",
    "category": "animal"
  },
  {
    "name": "cat2",
    "unicode": {"apple":"1F408", "google":"1F408", "twitter":"1F408"},
    "shortcode": "cat2",
    "description": "CAT",
    "category": "animal"
  },
  {
    "name": "rabbit2",
    "unicode": {"apple":"1F407", "google":"1F407", "twitter":"1F407"},
    "shortcode": "rabbit2",
    "description": "RABBIT",
    "category": "animal"
  },
  {
    "name": "chipmunk",
    "unicode": {"apple":"1F43F", "google":"1F43F", "twitter":"1F43F"},
    "shortcode": "chipmunk",
    "description": "Chipmunk",
    "category": "animal"
  },
  {
    "name": "feet",
    "unicode": {"apple":"1F43E", "google":"1F43E", "twitter":"1F43E"},
    "shortcode": "feet",
    "description": "PAW PRINTS",
    "category": "animal"
  },
  {
    "name": "dragon",
    "unicode": {"apple":"1F409", "google":"1F409", "twitter":"1F409"},
    "shortcode": "dragon",
    "description": "DRAGON",
    "category": "animal"
  },
  {
    "name": "dragon_face",
    "unicode": {"apple":"1F432", "google":"1F432", "twitter":"1F432"},
    "shortcode": "dragon_face",
    "description": "DRAGON FACE",
    "category": "animal"
  },
  {
    "name": "cactus",
    "unicode": {"apple":"1F335", "google":"1F335", "twitter":"1F335"},
    "shortcode": "cactus",
    "description": "CACTUS",
    "category": "animal"
  },
  {
    "name": "christmas_tree",
    "unicode": {"apple":"1F384", "google":"1F384", "twitter":"1F384"},
    "shortcode": "christmas_tree",
    "description": "CHRISTMAS TREE",
    "category": "nature"
  },
  {
    "name": "evergreen_tree",
    "unicode": {"apple":"1F332", "google":"1F332", "twitter":"1F332"},
    "shortcode": "evergreen_tree",
    "description": "EVERGREEN TREE",
    "category": "nature"
  },
  {
    "name": "deciduous_tree",
    "unicode": {"apple":"1F333", "google":"1F333", "twitter":"1F333"},
    "shortcode": "deciduous_tree",
    "description": "DECIDUOUS TREE",
    "category": "nature"
  },
  {
    "name": "palm_tree",
    "unicode": {"apple":"1F334", "google":"1F334", "twitter":"1F334"},
    "shortcode": "palm_tree",
    "description": "PALM TREE",
    "category": "nature"
  },
  {
    "name": "seedling",
    "unicode": {"apple":"1F331", "google":"1F331", "twitter":"1F331"},
    "shortcode": "seedling",
    "description": "SEEDLING",
    "category": "nature"
  },
  {
    "name": "herb",
    "unicode": {"apple":"1F33F", "google":"1F33F", "twitter":"1F33F"},
    "shortcode": "herb",
    "description": "HERB",
    "category": "nature"
  },
  {
    "name": "shamrock",
    "unicode": {"apple":"2618", "google":"2618", "twitter":"2618"},
    "shortcode": "shamrock",
    "description": "Shamrock",
    "category": "nature"
  },
  {
    "name": "four_leaf_clover",
    "unicode": {"apple":"1F340", "google":"1F340", "twitter":"1F340"},
    "shortcode": "four_leaf_clover",
    "description": "FOUR LEAF CLOVER",
    "category": "nature"
  },
  {
    "name": "bamboo",
    "unicode": {"apple":"1F38D", "google":"1F38D", "twitter":"1F38D"},
    "shortcode": "bamboo",
    "description": "PINE DECORATION",
    "category": "nature"
  },
  {
    "name": "tanabata_tree",
    "unicode": {"apple":"1F38B", "google":"1F38B", "twitter":"1F38B"},
    "shortcode": "tanabata_tree",
    "description": "TANABATA TREE",
    "category": "nature"
  },
  {
    "name": "leaves",
    "unicode": {"apple":"1F343", "google":"1F343", "twitter":"1F343"},
    "shortcode": "leaves",
    "description": "LEAF FLUTTERING IN WIND",
    "category": "nature"
  },
  {
    "name": "fallen_leaf",
    "unicode": {"apple":"1F342", "google":"1F342", "twitter":"1F342"},
    "shortcode": "fallen_leaf",
    "description": "FALLEN LEAF",
    "category": "nature"
  },
  {
    "name": "maple_leaf",
    "unicode": {"apple":"1F341", "google":"1F341", "twitter":"1F341"},
    "shortcode": "maple_leaf",
    "description": "MAPLE LEAF",
    "category": "nature"
  },
  {
    "name": "ear_of_rice",
    "unicode": {"apple":"1F33E", "google":"1F33E", "twitter":"1F33E"},
    "shortcode": "ear_of_rice",
    "description": "EAR OF RICE",
    "category": "nature"
  },
  {
    "name": "hibiscus",
    "unicode": {"apple":"1F33A", "google":"1F33A", "twitter":"1F33A"},
    "shortcode": "hibiscus",
    "description": "HIBISCUS",
    "category": "nature"
  },
  {
    "name": "sunflower",
    "unicode": {"apple":"1F33B", "google":"1F33B", "twitter":"1F33B"},
    "shortcode": "sunflower",
    "description": "SUNFLOWER",
    "category": "nature"
  },
  {
    "name": "rose",
    "unicode": {"apple":"1F339", "google":"1F339", "twitter":"1F339"},
    "shortcode": "rose",
    "description": "ROSE",
    "category": "nature"
  },
  {
    "name": "tulip",
    "unicode": {"apple":"1F337", "google":"1F337", "twitter":"1F337"},
    "shortcode": "tulip",
    "description": "TULIP",
    "category": "nature"
  },
  {
    "name": "blossom",
    "unicode": {"apple":"1F33C", "google":"1F33C", "twitter":"1F33C"},
    "shortcode": "blossom",
    "description": "BLOSSOM",
    "category": "nature"
  },
  {
    "name": "cherry_blossom",
    "unicode": {"apple":"1F338", "google":"1F338", "twitter":"1F338"},
    "shortcode": "cherry_blossom",
    "description": "CHERRY BLOSSOM",
    "category": "nature"
  },
  {
    "name": "bouquet",
    "unicode": {"apple":"1F490", "google":"1F490", "twitter":"1F490"},
    "shortcode": "bouquet",
    "description": "BOUQUET",
    "category": "nature"
  },
  {
    "name": "mushroom",
    "unicode": {"apple":"1F344", "google":"1F344", "twitter":"1F344"},
    "shortcode": "mushroom",
    "description": "MUSHROOM",
    "category": "nature"
  },
  {
    "name": "chestnut",
    "unicode": {"apple":"1F330", "google":"1F330", "twitter":"1F330"},
    "shortcode": "chestnut",
    "description": "CHESTNUT",
    "category": "nature"
  },
  {
    "name": "jack_o_lantern",
    "unicode": {"apple":"1F383", "google":"1F383", "twitter":"1F383"},
    "shortcode": "jack_o_lantern",
    "description": "JACK-O-LANTERN",
    "category": "nature"
  },
  {
    "name": "shell",
    "unicode": {"apple":"1F41A", "google":"1F41A", "twitter":"1F41A"},
    "shortcode": "shell",
    "description": "SPIRAL SHELL",
    "category": "nature"
  },
  {
    "name": "spider_web",
    "unicode": {"apple":"1F578", "google":"1F578", "twitter":"1F578"},
    "shortcode": "spider_web",
    "description": "Spider Web",
    "category": "nature"
  },
  {
    "name": "earth_americas",
    "unicode": {"apple":"1F30E", "google":"1F30E", "twitter":"1F30E"},
    "shortcode": "earth_americas",
    "description": "EARTH GLOBE AMERICAS",
    "category": "nature"
  },
  {
    "name": "earth_africa",
    "unicode": {"apple":"1F30D", "google":"1F30D", "twitter":"1F30D"},
    "shortcode": "earth_africa",
    "description": "EARTH GLOBE EUROPE-AFRICA",
    "category": "nature"
  },
  {
    "name": "earth_asia",
    "unicode": {"apple":"1F30F", "google":"1F30F", "twitter":"1F30F"},
    "shortcode": "earth_asia",
    "description": "EARTH GLOBE ASIA-AUSTRALIA",
    "category": "nature"
  },
  {
    "name": "full_moon",
    "unicode": {"apple":"1F315", "google":"1F315", "twitter":"1F315"},
    "shortcode": "full_moon",
    "description": "FULL MOON SYMBOL",
    "category": "nature"
  },
  {
    "name": "waning_gibbous_moon",
    "unicode": {"apple":"1F316", "google":"1F316", "twitter":"1F316"},
    "shortcode": "waning_gibbous_moon",
    "description": "WANING GIBBOUS MOON SYMBOL",
    "category": "nature"
  },
  {
    "name": "last_quarter_moon",
    "unicode": {"apple":"1F317", "google":"1F317", "twitter":"1F317"},
    "shortcode": "last_quarter_moon",
    "description": "LAST QUARTER MOON SYMBOL",
    "category": "nature"
  },
  {
    "name": "waning_crescent_moon",
    "unicode": {"apple":"1F318", "google":"1F318", "twitter":"1F318"},
    "shortcode": "waning_crescent_moon",
    "description": "WANING CRESCENT MOON SYMBOL",
    "category": "nature"
  },
 {
    "name": "new_moon",
    "unicode": {"apple":"1F311", "google":"1F311", "twitter":"1F311"},
    "shortcode": "new_moon",
    "description": "NEW MOON SYMBOL",
    "category": "nature"
  },
  {
    "name": "waxing_crescent_moon",
    "unicode": {"apple":"1F312", "google":"1F312", "twitter":"1F312"},
    "shortcode": "waxing_crescent_moon",
    "description": "WAXING CRESCENT MOON SYMBOL",
    "category": "nature"
  },
  {
    "name": "first_quarter_moon",
    "unicode": {"apple":"1F313", "google":"1F313", "twitter":"1F313"},
    "shortcode": "first_quarter_moon",
    "description": "FIRST QUARTER MOON SYMBOL",
    "category": "nature"
  },
  {
    "name": "moon",
    "unicode": {"apple":"1F314", "google":"1F314", "twitter":"1F314"},
    "shortcode": "moon",
    "description": "WAXING GIBBOUS MOON SYMBOL",
    "category": "nature"
  },
  {
    "name": "new_moon_with_face",
    "unicode": { "apple":"1F31A", "google":"1F31A", "twitter":"1F31A"},
    "shortcode": "new_moon_with_face",
    "description": "NEW MOON WITH FACE",
    "category": "nature"
  },
  {
    "name": "first_quarter_moon_with_face",
    "unicode": {"apple":"1F31B", "google":"1F31B", "twitter":"1F31B"},
    "shortcode": "first_quarter_moon_with_face",
    "description": "FIRST QUARTER MOON WITH FACE",
    "category": "nature"
  },
  {
    "name": "last_quarter_moon_with_face",
    "unicode": {"apple":"1F31C", "google":"1F31C", "twitter":"1F31C"},
    "shortcode": "last_quarter_moon_with_face",
    "description": "LAST QUARTER MOON WITH FACE",
    "category": "nature"
  },
  {
    "name": "full_moon_with_face",
    "unicode": {"apple":"1F31D", "google":"1F31D", "twitter":"1F31D"},
    "shortcode": "full_moon_with_face",
    "description": "FULL MOON WITH FACE",
    "category": "nature"
  },
  {
    "name": "crescent_moon",
    "unicode": {"apple":"1F319", "google":"1F319", "twitter":"1F319"},
    "shortcode": "crescent_moon",
    "description": "CRESCENT MOON",
    "category": "nature"
  },

  {
    "name": "sun_with_face",
    "unicode": {"apple":"1F31E", "google":"1F31E", "twitter":"1F31E"},
    "shortcode": "sun_with_face",
    "description": "SUN WITH FACE",
    "category": "nature"
  },
  {
    "name": "star",
    "unicode": {"apple":"2B50", "google":"2B50", "twitter":"2B50"},
    "shortcode": "star",
    "description": "WHITE MEDIUM STAR",
    "category": "nature"
  },
  {
    "name": "star2",
    "unicode": {"apple":"1F31F", "google":"1F31F", "twitter":"1F31F"},
    "shortcode": "star2",
    "description": "GLOWING STAR",
    "category": "nature"
  },
  {
    "name": "dizzy",
    "unicode": {"apple":"1F4AB", "google":"1F4AB", "twitter":"1F4AB"},
    "shortcode": "dizzy",
    "description": "DIZZY SYMBOL",
    "category": "nature"
  },
  {
    "name": "sparkles",
    "unicode": {"apple":"2728", "google":"2728", "twitter":"2728"},
    "shortcode": "sparkles",
    "description": "SPARKLES",
    "category": "nature"
  },
  {
    "name": "comet",
    "unicode": {"apple":"2604", "google":"2604", "twitter":"2604"},
    "shortcode": "comet",
    "description": "Comet",
    "category": "nature"
  },
  {
    "name": "sunny",
    "unicode": {"apple":"2600", "google":"2600", "twitter":"2600"},
    "shortcode": "sunny",
    "description": "BLACK SUN WITH RAYS",
    "category": "nature"
  },
  {
    "name": "mostly_sunny",
    "unicode": {"apple":"1F324", "google":"1F324", "twitter":"1F324"},
    "shortcode": "mostly_sunny",
    "description": "White Sun With Small Cloud",
    "category": "nature"
  },
  {
    "name": "partly_sunny",
    "unicode": {"apple":"26C5", "google":"26C5", "twitter":"26C5"},
    "shortcode": "partly_sunny",
    "description": "SUN BEHIND CLOUD",
    "category": "nature"
  },
  {
    "name": "barely_sunny",
    "unicode": {"apple":"1F325", "google":"1F325", "twitter":"1F325"},
    "shortcode": "barely_sunny",
    "description": "White Sun Behind Cloud",
    "category": "nature"
  },
  {
    "name": "partly_sunny_rain",
    "unicode": {"apple":"1F326", "google":"1F326", "twitter":"1F326"},
    "shortcode": "partly_sunny_rain",
    "description": "White Sun Behind Cloud With Rain",
    "category": "nature"
  },
  {
    "name": "cloud",
    "unicode": {"apple":"2601", "google":"2601", "twitter":"2601"},
    "shortcode": "cloud",
    "description": "CLOUD",
    "category": "nature"
  },
  {
    "name": "rain_cloud",
    "unicode": {"apple":"1F327", "google":"1F327", "twitter":"1F327"},
    "shortcode": "rain_cloud",
    "description": "Cloud With Rain",
    "category": "nature"
  },
  {
    "name": "thunder_cloud_and_rain",
    "unicode": {"apple":"26C8", "google":"26C8", "twitter":"26C8"},
    "shortcode": "thunder_cloud_and_rain",
    "description": "Thunder Cloud and Rain",
    "category": "nature"
  },
  {
    "name": "lightning",
    "unicode": {"apple":"1F329", "google":"1F329", "twitter":"1F329"},
    "shortcode": "lightning",
    "description": "Cloud With Lightning",
    "category": "nature"
  },
  {
    "name": "zap",
    "unicode": {"apple":"26A1", "google":"26A1", "twitter":"26A1"},
    "shortcode": "zap",
    "description": "HIGH VOLTAGE SIGN",
    "category": "nature"
  },
  {
    "name": "fire",
    "unicode": {"apple":"1F525", "google":"1F525", "twitter":"1F525"},
    "shortcode": "fire",
    "description": "FIRE",
    "category": "nature"
  },
  {
    "name": "boom",
    "unicode": {"apple":"1F4A5", "google":"1F4A5", "twitter":"1F4A5"},
    "shortcode": "boom",
    "description": "COLLISION SYMBOL",
    "category": "nature"
  },
  {
    "name": "snowflake",
    "unicode": {"apple":"2744", "google":"2744", "twitter":"2744"},
    "shortcode": "snowflake",
    "description": "SNOWFLAKE",
    "category": "nature"
  },
  {
    "name": "snow_cloud",
    "unicode": {"apple":"1F328", "google":"1F328", "twitter":"1F328"},
    "shortcode": "snow_cloud",
    "description": "Cloud With Snow",
    "category": "nature"
  },
  {
    "name": "snowman",
    "unicode": {"apple":"26C4", "google":"26C4", "twitter":"26C4"},
    "shortcode": "snowman",
    "description": "SNOWMAN WITHOUT SNOW",
    "category": "nature"
  },
  {
    "name": "wind_blowing_face",
    "unicode": {"apple":"1F32C", "google":"1F32C", "twitter":"1F32C"},
    "shortcode": "wind_blowing_face",
    "description": "Wind Blowing Face",
    "category": "nature"
  },
  {
    "name": "dash",
    "unicode": {"apple":"1F4A8", "google":"1F4A8", "twitter":"1F4A8"},
    "shortcode": "dash",
    "description": "DASH SYMBOL",
    "category": "nature"
  },
  {
    "name": "tornado",
    "unicode": {"apple":"1F32A", "google":"1F32A", "twitter":"1F32A"},
    "shortcode": "tornado",
    "description": "Cloud With Tornado",
    "category": "nature"
  },
  {
    "name": "fog",
    "unicode": {"apple":"1F32B", "google":"1F32B", "twitter":"1F32B"},
    "shortcode": "fog",
    "description": "Fog",
    "category": "nature"
  },
  {
    "name": "umbrella",
    "unicode": {"apple":"2614", "google":"2614", "twitter":"2614"},
    "shortcode": "umbrella",
    "description": "UMBRELLA WITH RAIN DROPS",
    "category": "nature"
  },
  {
    "name": "droplet",
    "unicode": {"apple":"1F4A7", "google":"1F4A7", "twitter":"1F4A7"},
    "shortcode": "droplet",
    "description": "DROPLET",
    "category": "nature"
  },
  {
    "name": "sweat_drops",
    "unicode": {"apple":"1F4A6", "google":"1F4A6", "twitter":"1F4A6"},
    "shortcode": "sweat_drops",
    "description": "SPLASHING SWEAT SYMBOL",
    "category": "nature"
  },
  {
    "name": "ocean",
    "unicode": {"apple":"1F30A", "google":"1F30A", "twitter":"1F30A"},
    "shortcode": "ocean",
    "description": "WATER WAVE",
    "category": "nature"
  },
  {
    "name": "green_apple",
    "unicode": {"apple":"1F34F", "google":"1F34F", "twitter":"1F34F"},
    "shortcode": "green_apple",
    "description": "GREEN APPLE",
    "category": "food"
  },
  {
    "name": "apple",
    "unicode": {"apple":"1F34E", "google":"1F34E", "twitter":"1F34E"},
    "shortcode": "apple",
    "description": "RED APPLE",
    "category": "food"
  },
  {
    "name": "pear",
    "unicode": {"apple":"1F350", "google":"1F350", "twitter":"1F350"},
    "shortcode": "pear",
    "description": "PEAR",
    "category": "food"
  },
  {
    "name": "tangerine",
    "unicode": {"apple":"1F34A", "google":"1F34A", "twitter":"1F34A"},
    "shortcode": "tangerine",
    "description": "TANGERINE",
    "category": "food"
  },
  {
    "name": "lemon",
    "unicode": {"apple":"1F34B", "google":"1F34B", "twitter":"1F34B"},
    "shortcode": "lemon",
    "description": "LEMON",
    "category": "food"
  },
  {
    "name": "banana",
    "unicode": {"apple":"1F34C", "google":"1F34C", "twitter":"1F34C"},
    "shortcode": "banana",
    "description": "BANANA",
    "category": "food"
  },
  {
    "name": "watermelon",
    "unicode": {"apple":"1F349", "google":"1F349", "twitter":"1F349"},
    "shortcode": "watermelon",
    "description": "WATERMELON",
    "category": "food"
  },
  {
    "name": "grapes",
    "unicode": {"apple":"1F347", "google":"1F347", "twitter":"1F347"},
    "shortcode": "grapes",
    "description": "GRAPES",
    "category": "food"
  },
  {
    "name": "strawberry",
    "unicode": {"apple":"1F353", "google":"1F353", "twitter":"1F353"},
    "shortcode": "strawberry",
    "description": "STRAWBERRY",
    "category": "food"
  },
  {
    "name": "melon",
    "unicode": {"apple":"1F348", "google":"1F348", "twitter":"1F348"},
    "shortcode": "melon",
    "description": "MELON",
    "category": "food"
  },
  {
    "name": "cherries",
    "unicode": {"apple":"1F352", "google":"1F352", "twitter":"1F352"},
    "shortcode": "cherries",
    "description": "CHERRIES",
    "category": "food"
  },
  {
    "name": "peach",
    "unicode": {"apple":"1F351", "google":"1F351", "twitter":"1F351"},
    "shortcode": "peach",
    "description": "PEACH",
    "category": "food"
  },
  {
    "name": "pineapple",
    "unicode": {"apple":"1F34D", "google":"1F34D", "twitter":"1F34D"},
    "shortcode": "pineapple",
    "description": "PINEAPPLE",
    "category": "food"
  },
  {
    "name": "tomato",
    "unicode": {"apple":"1F345", "google":"1F345", "twitter":"1F345"},
    "shortcode": "tomato",
    "description": "TOMATO",
    "category": "food"
  },
  {
    "name": "eggplant",
    "unicode": {"apple":"1F346", "google":"1F346", "twitter":"1F346"},
    "shortcode": "eggplant",
    "description": "AUBERGINE",
    "category": "food"
  },
  {
    "name": "hot_pepper",
    "unicode": {"apple":"1F336", "google":"1F336", "twitter":"1F336"},
    "shortcode": "hot_pepper",
    "description": "Hot Pepper",
    "category": "food"
  },
  {
    "name": "corn",
    "unicode": {"apple":"1F33D", "google":"1F33D", "twitter":"1F33D"},
    "shortcode": "corn",
    "description": "EAR OF MAIZE",
    "category": "thing"
  },
  {
    "name": "sweet_potato",
    "unicode": {"apple":"1F360", "google":"1F360", "twitter":"1F360"},
    "shortcode": "sweet_potato",
    "description": "ROASTED SWEET POTATO",
    "category": "food"
  },
  {
    "name": "honey_pot",
    "unicode": {"apple":"1F36F", "google":"1F36F", "twitter":"1F36F"},
    "shortcode": "honey_pot",
    "description": "HONEY POT",
    "category": "food"
  },
  {
    "name": "bread",
    "unicode": {"apple":"1F35E", "google":"1F35E", "twitter":"1F35E"},
    "shortcode": "bread",
    "description": "BREAD",
    "category": "food"
  },
  {
    "name": "cheese_wedge",
    "unicode": {"apple":"1F9C0", "google":"1F9C0", "twitter":"1F9C0"},
    "shortcode": "cheese_wedge",
    "description": "Cheese Wedge",
    "category": "food"
  },
  {
    "name": "poultry_leg",
    "unicode": {"apple":"1F357", "google":"1F357", "twitter":"1F357"},
    "shortcode": "poultry_leg",
    "description": "POULTRY LEG",
    "category": "food"
  },
  {
    "name": "meat_on_bone",
    "unicode": {"apple":"1F356", "google":"1F356", "twitter":"1F356"},
    "shortcode": "meat_on_bone",
    "description": "MEAT ON BONE",
    "category": "food"
  },
  {
    "name": "fried_shrimp",
    "unicode": {"apple":"1F364", "google":"1F364", "twitter":"1F364"},
    "shortcode": "fried_shrimp",
    "description": "FRIED SHRIMP",
    "category": "food"
  },
  {
    "name": "egg",
    "unicode": {"apple":"1F373", "google":"1F373", "twitter":"1F373"},
    "shortcode": "egg",
    "description": "COOKING",
    "category": "food"
  },
  {
    "name": "hamburger",
    "unicode": {"apple":"1F354", "google":"1F354", "twitter":"1F354"},
    "shortcode": "hamburger",
    "description": "HAMBURGER",
    "category": "food"
  },
  {
    "name": "fries",
    "unicode": {"apple":"1F35F", "google":"1F35F", "twitter":"1F35F"},
    "shortcode": "fries",
    "description": "FRENCH FRIES",
    "category": "food"
  },
  {
    "name": "hotdog",
    "unicode": {"apple":"1F32D", "google":"1F32D", "twitter":"1F32D"},
    "shortcode": "hotdog",
    "description": "Hot Dog",
    "category": "food"
  },
  {
    "name": "pizza",
    "unicode": {"apple":"1F355", "google":"1F355", "twitter":"1F355"},
    "shortcode": "pizza",
    "description": "SLICE OF PIZZA",
    "category": "food"
  },
  {
    "name": "spaghetti",
    "unicode": {"apple":"1F35D", "google":"1F35D", "twitter":"1F35D"},
    "shortcode": "spaghetti",
    "description": "SPAGHETTI",
    "category": "food"
  },
  {
    "name": "taco",
    "unicode": {"apple":"1F32E", "google":"1F32E", "twitter":"1F32E"},
    "shortcode": "taco",
    "description": "Taco",
    "category": "food"
  },
  {
    "name": "burrito",
    "unicode": {"apple":"1F32F", "google":"1F32F", "twitter":"1F32F"},
    "shortcode": "burrito",
    "description": "Burrito",
    "category": "food"
  },
  {
    "name": "ramen",
    "unicode": {"apple":"1F35C", "google":"1F35C", "twitter":"1F35C"},
    "shortcode": "ramen",
    "description": "STEAMING BOWL",
    "category": "food"
  },
  {
    "name": "stew",
    "unicode": {"apple":"1F372", "google":"1F372", "twitter":"1F372"},
    "shortcode": "stew",
    "description": "POT OF FOOD",
    "category": "food"
  },
  {
    "name": "fish_cake",
    "unicode": {"apple":"1F365", "google":"1F365", "twitter":"1F365"},
    "shortcode": "fish_cake",
    "description": "FISH CAKE WITH SWIRL DESIGN",
    "category": "food"
  },
  {
    "name": "sushi",
    "unicode": {"apple":"1F363", "google":"1F363", "twitter":"1F363"},
    "shortcode": "sushi",
    "description": "SUSHI",
    "category": "food"
  },
  {
    "name": "bento",
    "unicode": {"apple":"1F371", "google":"1F371", "twitter":"1F371"},
    "shortcode": "bento",
    "description": "BENTO BOX",
    "category": "food"
  },
  {
    "name": "curry",
    "unicode": {"apple":"1F35B", "google":"1F35B", "twitter":"1F35B"},
    "shortcode": "curry",
    "description": "CURRY AND RICE",
    "category": "food"
  },
  {
    "name": "rice_ball",
    "unicode": {"apple":"1F359", "google":"1F359", "twitter":"1F359"},
    "shortcode": "rice_ball",
    "description": "RICE BALL",
    "category": "food"
  },
  {
    "name": "rice",
    "unicode": {"apple":"1F35A", "google":"1F35A", "twitter":"1F35A"},
    "shortcode": "rice",
    "description": "COOKED RICE",
    "category": "food"
  },
  {
    "name": "rice_cracker",
    "unicode": {"apple":"1F358", "google":"1F358", "twitter":"1F358"},
    "shortcode": "rice_cracker",
    "description": "RICE CRACKER",
    "category": "food"
  },
  {
    "name": "oden",
    "unicode": {"apple":"1F362", "google":"1F362", "twitter":"1F362"},
    "shortcode": "oden",
    "description": "ODEN",
    "category": "food"
  },
  {
    "name": "dango",
    "unicode": {"apple":"1F361", "google":"1F361", "twitter":"1F361"},
    "shortcode": "dango",
    "description": "DANGO",
    "category": "food"
  },
  {
    "name": "shaved_ice",
    "unicode": {"apple":"1F367", "google":"1F367", "twitter":"1F367"},
    "shortcode": "shaved_ice",
    "description": "SHAVED ICE",
    "category": "food"
  },
  {
    "name": "ice_cream",
    "unicode": {"apple":"1F368", "google":"1F368", "twitter":"1F368"},
    "shortcode": "ice_cream",
    "description": "ICE CREAM",
    "category": "food"
  },
  {
    "name": "icecream",
    "unicode": {"apple":"1F366", "google":"1F366", "twitter":"1F366"},
    "shortcode": "icecream",
    "description": "SOFT ICE CREAM",
    "category": "food"
  },
  {
    "name": "cake",
    "unicode": {"apple":"1F370", "google":"1F370", "twitter":"1F370"},
    "shortcode": "cake",
    "description": "SHORTCAKE",
    "category": "food"
  },
  {
    "name": "birthday",
    "unicode": {"apple":"1F382", "google":"1F382", "twitter":"1F382"},
    "shortcode": "birthday",
    "description": "BIRTHDAY CAKE",
    "category": "food"
  },
  {
    "name": "custard",
    "unicode": {"apple":"1F36E", "google":"1F36E", "twitter":"1F36E"},
    "shortcode": "custard",
    "description": "CUSTARD",
    "category": "food"
  },
  {
    "name": "candy",
    "unicode": {"apple":"1F36C", "google":"1F36C", "twitter":"1F36C"},
    "shortcode": "candy",
    "description": "CANDY",
    "category": "food"
  },
  {
    "name": "lollipop",
    "unicode": {"apple":"1F36D", "google":"1F36D", "twitter":"1F36D"},
    "shortcode": "lollipop",
    "description": "LOLLIPOP",
    "category": "food"
  },
  {
    "name": "chocolate_bar",
    "unicode": {"apple":"1F36B", "google":"1F36B", "twitter":"1F36B"},
    "shortcode": "chocolate_bar",
    "description": "CHOCOLATE BAR",
    "category": "food"
  },
  {
    "name": "popcorn",
    "unicode": {"apple":"1F37F", "google":"1F37F", "twitter":"1F37F"},
    "shortcode": "popcorn",
    "description": "Popcorn",
    "category": "food"
  },
  {
    "name": "cookie",
    "unicode": {"apple":"1F36A", "google":"1F36A", "twitter":"1F36A"},
    "shortcode": "cookie",
    "description": "COOKIE",
    "category": "food"
  },
  {
    "name": "doughnut",
    "unicode": {"apple":"1F369", "google":"1F369", "twitter":"1F369"},
    "shortcode": "doughnut",
    "description": "DOUGHNUT",
    "category": "food"
  },
  {
    "name": "beer",
    "unicode": {"apple":"1F37A", "google":"1F37A", "twitter":"1F37A"},
    "shortcode": "beer",
    "description": "BEER MUG",
    "category": "food"
  },
  {
    "name": "beers",
    "unicode": {"apple":"1F37B", "google":"1F37B", "twitter":"1F37B"},
    "shortcode": "beers",
    "description": "CLINKING BEER MUGS",
    "category": "food"
  },
  {
    "name": "wine_glass",
    "unicode": {"apple":"1F377", "google":"1F377", "twitter":"1F377"},
    "shortcode": "wine_glass",
    "description": "WINE GLASS",
    "category": "food"
  },
  {
    "name": "cocktail",
    "unicode": {"apple":"1F378", "google":"1F378", "twitter":"1F378"},
    "shortcode": "cocktail",
    "description": "COCKTAIL GLASS",
    "category": "food"
  },
  {
    "name": "champagne",
    "unicode": {"apple":"1F379", "google":"1F379", "twitter":"1F379"},
    "shortcode": "tropical_drink",
    "description": "TROPICAL DRINK",
    "category": "food"
  },
  {
    "name": "champagne",
    "unicode": {"apple":"1F37E", "google":"1F37E", "twitter":"1F37E"},
    "shortcode": "champagne",
    "description": "Bottle With Popping Cork",
    "category": "food"
  },
  {
    "name": "sake",
    "unicode": {"apple":"1F376", "google":"1F376", "twitter":"1F376"},
    "shortcode": "sake",
    "description": "SAKE BOTTLE AND CUP",
    "category": "food"
  },
  {
    "name": "tea",
    "unicode": {"apple":"1F375", "google":"1F375", "twitter":"1F375"},
    "shortcode": "tea",
    "description": "TEACUP WITHOUT HANDLE",
    "category": "food"
  },
  {
    "name": "coffee",
    "unicode": {"apple":"2615", "google":"2615", "twitter":"2615"},
    "shortcode": "coffee",
    "description": "HOT BEVERAGE",
    "category": "food"
  },
  {
    "name": "baby_bottle",
    "unicode": {"apple":"1F37C", "google":"1F37C", "twitter":"1F37C"},
    "shortcode": "baby_bottle",
    "description": "BABY BOTTLE",
    "category": "food"
  },
  {
    "name": "fork_and_knife",
    "unicode": {"apple":"1F374", "google":"1F374", "twitter":"1F374"},
    "shortcode": "fork_and_knife",
    "description": "FORK AND KNIFE",
    "category": "food"
  },
  {
    "name": "knife_fork_plate",
    "unicode": {"apple":"1F37D", "google":"1F37D", "twitter":"1F37D"},
    "shortcode": "knife_fork_plate",
    "description": "Fork and Knife With Plate",
    "category": "food"
  },
  {
    "name": "soccer",
    "unicode": {"apple":"26BD", "google":"26BD", "twitter":"26BD"},
    "shortcode": "soccer",
    "description": "SOCCER BALL",
    "category": "activity"
  },
  {
    "name": "basketball",
    "unicode": {"apple":"1F3C0", "google":"1F3C0", "twitter":"1F3C0"},
    "shortcode": "basketball",
    "description": "BASKETBALL AND HOOP",
    "category": "activity"
  },
  {
    "name": "football",
    "unicode": {"apple":"1F3C8", "google":"1F3C8", "twitter":"1F3C8"},
    "shortcode": "football",
    "description": "AMERICAN FOOTBALL",
    "category": "activity"
  },
  {
    "name": "baseball",
    "unicode": {"apple":"26BE", "google":"26BE", "twitter":"26BE"},
    "shortcode": "baseball",
    "description": "BASEBALL",
    "category": "activity"
  },
  {
    "name": "tennis",
    "unicode": {"apple":"1F3BE", "google":"1F3BE", "twitter":"1F3BE"},
    "shortcode": "tennis",
    "description": "TENNIS RACQUET AND BALL",
    "category": "activity"
  },
  {
    "name": "volleyball",
    "unicode": {"apple":"1F3D0", "google":"1F3D0", "twitter":"1F3D0"},
    "shortcode": "volleyball",
    "description": "Volleyball",
    "category": "activity"
  },
  {
    "name": "rugby_football",
    "unicode": {"apple":"1F3C9", "google":"1F3C9", "twitter":"1F3C9"},
    "shortcode": "rugby_football",
    "description": "RUGBY FOOTBALL",
    "category": "activity"
  },
  {
    "name": "8ball",
    "unicode": {"apple":"1F3B1", "google":"1F3B1", "twitter":"1F3B1"},
    "shortcode": "8ball",
    "description": "BILLIARDS",
    "category": "activity"
  },
  {
    "name": "golf",
    "unicode": {"apple":"26F3", "google":"26F3", "twitter":"26F3"},
    "shortcode": "golf",
    "description": "FLAG IN HOLE",
    "category": "activity"
  },
  {
    "name": "golfer",
    "unicode": {"apple":"1F3CC", "google":"1F3CC", "twitter":"1F3CC"},
    "shortcode": "golfer",
    "description": "Golfer",
    "category": "activity"
  },
  {
    "name": "table_tennis_paddle_and_ball",
    "unicode": {"apple":"1F3D3", "google":"1F3D3", "twitter":"1F3D3"},
    "shortcode": "table_tennis_paddle_and_ball",
    "description": "Table Tennis Paddle and Ball",
    "category": "activity"
  },
  {
    "name": "badminton_racquet_and_shuttlecock",
    "unicode": {"apple":"1F3F8", "google":"1F3F8", "twitter":"1F3F8"},
    "shortcode": "badminton_racquet_and_shuttlecock",
    "description": "Badminton Racquet and Shuttlecock",
    "category": "activity"
  },
  {
    "name": "ice_hockey_stick_and_puck",
    "unicode": {"apple":"1F3D2", "google":"1F3D2", "twitter":"1F3D2"},
    "shortcode": "ice_hockey_stick_and_puck",
    "description": "Ice Hockey Stick and Puck",
    "category": "activity"
  },
  {
    "name": "field_hockey_stick_and_ball",
    "unicode": {"apple":"1F3D1", "google":"1F3D1", "twitter":"1F3D1"},
    "shortcode": "field_hockey_stick_and_ball",
    "description": "Field Hockey Stick and Ball",
    "category": "activity"
  },
  {
    "name": "cricket_bat_and_ball",
    "unicode": {"apple":"1F3CF", "google":"1F3CF", "twitter":"1F3CF"},
    "shortcode": "cricket_bat_and_ball",
    "description": "Cricket Bat and Ball",
    "category": "activity"
  },
  {
    "name": "ski",
    "unicode": {"apple":"1F3BF", "google":"1F3BF", "twitter":"1F3BF"},
    "shortcode": "ski",
    "description": "SKI AND SKI BOOT",
    "category": "activity"
  },
  {
    "name": "skier",
    "unicode": {"apple":"26F7", "google":"26F7", "twitter":"26F7"},
    "shortcode": "skier",
    "description": "Skier",
    "category": "activity"
  },
  {
    "name": "snowboarder",
    "unicode": {"apple":"1F3C2", "google":"1F3C2", "twitter":"1F3C2"},
    "shortcode": "snowboarder",
    "description": "SNOWBOARDER",
    "category": "activity"
  },
  {
    "name": "ice_skate",
    "unicode": {"apple":"26F8", "google":"26F8", "twitter":"26F8"},
    "shortcode": "ice_skate",
    "description": "Ice Skate",
    "category": "activity"
  },
  {
    "name": "bow_and_arrow",
    "unicode": {"apple":"1F3F9", "google":"1F3F9", "twitter":"1F3F9"},
    "shortcode": "bow_and_arrow",
    "description": "Bow and Arrow",
    "category": "activity"
  },
  {
    "name": "fishing_pole_and_fish",
    "unicode": {"apple":"1F3A3", "google":"1F3A3", "twitter":"1F3A3"},
    "shortcode": "fishing_pole_and_fish",
    "description": "FISHING POLE AND FISH",
    "category": "activity"
  },
  {
    "name": "rowboat",
    "unicode": {"apple":"1F6A3", "google":"1F6A3", "twitter":"1F6A3"},
    "shortcode": "rowboat",
    "description": "ROWBOAT",
    "category": "activity"
  },
  {
    "name": "swimmer",
    "unicode": {"apple":"1F3CA", "google":"1F3CA", "twitter":"1F3CA"},
    "shortcode": "swimmer",
    "description": "SWIMMER",
    "category": "activity"
  },
  {
    "name": "surfer",
    "unicode": {"apple":"1F3C4", "google":"1F3C4", "twitter":"1F3C4"},
    "shortcode": "surfer",
    "description": "SURFER",
    "category": "activity"
  },
  {
    "name": "bath",
    "unicode": {"apple":"1F6C0", "google":"1F6C0", "twitter":"1F6C0"},
    "shortcode": "bath",
    "description": "BATH",
    "category": "activity"
  },
  {
    "name": "person_with_ball",
    "unicode": {"apple":"26F9", "google":"26F9", "twitter":"26F9"},
    "shortcode": "person_with_ball",
    "description": "Person With Ball",
    "category": "activity"
  },
  {
    "name": "weight_lifter",
    "unicode": {"apple":"1F3CB", "google":"1F3CB", "twitter":"1F3CB"},
    "shortcode": "weight_lifter",
    "description": "Weight Lifter",
    "category": "activity"
  },
  {
    "name": "bicyclist",
    "unicode": {"apple":"1F6B4", "google":"1F6B4", "twitter":"1F6B4"},
    "shortcode": "bicyclist",
    "description": "BICYCLIST",
    "category": "activity"
  },
  {
    "name": "mountain_bicyclist",
    "unicode": {"apple":"1F6B5", "google":"1F6B5", "twitter":"1F6B5"},
    "shortcode": "mountain_bicyclist",
    "description": "MOUNTAIN BICYCLIST",
    "category": "activity"
  },
  {
    "name": "horse_racing",
    "unicode": {"apple":"1F3C7", "google":"1F3C7", "twitter":"1F3C7"},
    "shortcode": "horse_racing",
    "description": "HORSE RACING",
    "category": "activity"
  },
  {
    "name": "man_in_business_suit_levitating",
    "unicode": {"apple":"1F574", "google":"1F574", "twitter":"1F574"},
    "shortcode": "man_in_business_suit_levitating",
    "description": "Man in Business Suit Levitating",
    "category": "activity"
  },
  {
    "name": "trophy",
    "unicode": {"apple":"1F3C6", "google":"1F3C6", "twitter":"1F3C6"},
    "shortcode": "trophy",
    "description": "TROPHY",
    "category": "activity"
  },
  {
    "name": "running_shirt_with_sash",
    "unicode": {"apple":"1F3BD", "google":"1F3BD", "twitter":"1F3BD"},
    "shortcode": "running_shirt_with_sash",
    "description": "RUNNING SHIRT WITH SASH",
    "category": "activity"
  },
  {
    "name": "sports_medal",
    "unicode": {"apple":"1F3C5", "google":"1F3C5", "twitter":"1F3C5"},
    "shortcode": "sports_medal",
    "description": "Sports Medal",
    "category": "activity"
  },
  {
    "name": "medal",
    "unicode": {"apple":"1F396", "google":"1F396", "twitter":"1F396"},
    "shortcode": "medal",
    "description": "Military Medal",
    "category": "activity"
  },
  {
    "name": "reminder_ribbon",
    "unicode": {"apple":"1F397", "google":"1F397", "twitter":"1F397"},
    "shortcode": "reminder_ribbon",
    "description": "Reminder Ribbon",
    "category": "activity"
  },
  {
    "name": "rosette",
    "unicode": {"apple":"1F3F5", "google":"1F3F5", "twitter":"1F3F5"},
    "shortcode": "rosette",
    "description": "Rosette",
    "category": "activity"
  },
  {
    "name": "ticket",
    "unicode": {"apple":"1F3AB", "google":"1F3AB", "twitter":"1F3AB"},
    "shortcode": "ticket",
    "description": "TICKET",
    "category": "activity"
  },
  {
    "name": "admission_tickets",
    "unicode": {"apple":"1F39F", "google":"1F39F", "twitter":"1F39F"},
    "shortcode": "admission_tickets",
    "description": "Admission Tickets",
    "category": "activity"
  },
  {
    "name": "performing_arts",
    "unicode": {"apple":"1F3AD", "google":"1F3AD", "twitter":"1F3AD"},
    "shortcode": "performing_arts",
    "description": "PERFORMING ARTS",
    "category": "activity"
  },
  {
    "name": "art",
    "unicode": {"apple":"1F3A8", "google":"1F3A8", "twitter":"1F3A8"},
    "shortcode": "art",
    "description": "ARTIST PALETTE",
    "category": "activity"
  },
  {
    "name": "circus_tent",
    "unicode": {"apple":"1F3AA", "google":"1F3AA", "twitter":"1F3AA"},
    "shortcode": "circus_tent",
    "description": "CIRCUS TENT",
    "category": "activity"
  },
  {
    "name": "microphone",
    "unicode": {"apple":"1F3A4", "google":"1F3A4", "twitter":"1F3A4"},
    "shortcode": "microphone",
    "description": "MICROPHONE",
    "category": "activity"
  },
  {
    "name": "headphones",
    "unicode": {"apple":"1F3A7", "google":"1F3A7", "twitter":"1F3A7"},
    "shortcode": "headphones",
    "description": "HEADPHONE",
    "category": "activity"
  },
  {
    "name": "musical_score",
    "unicode": {"apple":"1F3BC", "google":"1F3BC", "twitter":"1F3BC"},
    "shortcode": "musical_score",
    "description": "MUSICAL SCORE",
    "category": "activity"
  },
  {
    "name": "musical_keyboard",
    "unicode": {"apple":"1F3B9", "google":"1F3B9", "twitter":"1F3B9"},
    "shortcode": "musical_keyboard",
    "description": "MUSICAL KEYBOARD",
    "category": "activity"
  },
  {
    "name": "saxophone",
    "unicode": {"apple":"1F3B7", "google":"1F3B7", "twitter":"1F3B7"},
    "shortcode": "saxophone",
    "description": "SAXOPHONE",
    "category": "activity"
  },
  {
    "name": "trumpet",
    "unicode": {"apple":"1F3BA", "google":"1F3BA", "twitter":"1F3BA"},
    "shortcode": "trumpet",
    "description": "TRUMPET",
    "category": "activity"
  },
  {
    "name": "guitar",
    "unicode": {"apple":"1F3B8", "google":"1F3B8", "twitter":"1F3B8"},
    "shortcode": "guitar",
    "description": "GUITAR",
    "category": "activity"
  },
  {
    "name": "violin",
    "unicode": {"apple":"1F3BB", "google":"1F3BB", "twitter":"1F3BB"},
    "shortcode": "violin",
    "description": "VIOLIN",
    "category": "activity"
  },
  {
    "name": "clapper",
    "unicode": {"apple":"1F3AC", "google":"1F3AC", "twitter":"1F3AC"},
    "shortcode": "clapper",
    "description": "CLAPPER BOARD",
    "category": "activity"
  },
  {
    "name": "video_game",
    "unicode": {"apple":"1F3AE", "google":"1F3AE", "twitter":"1F3AE"},
    "shortcode": "video_game",
    "description": "VIDEO GAME",
    "category": "activity"
  },
  {
    "name": "space_invader",
    "unicode": {"apple":"1F47E", "google":"1F47E", "twitter":"1F47E"},
    "shortcode": "space_invader",
    "description": "ALIEN MONSTER",
    "category": "activity"
  },
  {
    "name": "dart",
    "unicode": {"apple":"1F3AF", "google":"1F3AF", "twitter":"1F3AF"},
    "shortcode": "dart",
    "description": "DIRECT HIT",
    "category": "activity"
  },
  {
    "name": "game_die",
    "unicode": {"apple":"1F3B2", "google":"1F3B2", "twitter":"1F3B2"},
    "shortcode": "game_die",
    "description": "GAME DIE",
    "category": "activity"
  },
  {
    "name": "slot_machine",
    "unicode": {"apple":"1F3B0", "google":"1F3B0", "twitter":"1F3B0"},
    "shortcode": "slot_machine",
    "description": "SLOT MACHINE",
    "category": "activity"
  },
  {
    "name": "bowling",
    "unicode": {"apple":"1F3B3", "google":"1F3B3", "twitter":"1F3B3"},
    "shortcode": "bowling",
    "description": "BOWLING",
    "category": "activity"
  },
  {
    "name": "car",
    "unicode": {"apple":"1F697", "google":"1F697", "twitter":"1F697"},
    "shortcode": "car",
    "description": "AUTOMOBILE",
    "category": "travel"
  },
  {
    "name": "taxi",
    "unicode": {"apple":"1F695", "google":"1F695", "twitter":"1F695"},
    "shortcode": "taxi",
    "description": "TAXI",
    "category": "travel"
  },
  {
    "name": "blue_car",
    "unicode": {"apple":"1F699", "google":"1F699", "twitter":"1F699"},
    "shortcode": "blue_car",
    "description": "RECREATIONAL VEHICLE",
    "category": "travel"
  },
  {
    "name": "bus",
    "unicode": {"apple":"1F68C", "google":"1F68C", "twitter":"1F68C"},
    "shortcode": "bus",
    "description": "BUS",
    "category": "travel"
  },
  {
    "name": "trolleybus",
    "unicode": {"apple":"1F68E", "google":"1F68E", "twitter":"1F68E"},
    "shortcode": "trolleybus",
    "description": "TROLLEYBUS",
    "category": "travel"
  },
  {
    "name": "racing_car",
    "unicode": {"apple":"1F3CE", "google":"1F3CE", "twitter":"1F3CE"},
    "shortcode": "racing_car",
    "description": "Racing Car",
    "category": "travel"
  },
  {
    "name": "police_car",
    "unicode": {"apple":"1F693", "google":"1F693", "twitter":"1F693"},
    "shortcode": "police_car",
    "description": "POLICE CAR",
    "category": "travel"
  },
  {
    "name": "ambulance",
    "unicode": {"apple":"1F691", "google":"1F691", "twitter":"1F691"},
    "shortcode": "ambulance",
    "description": "AMBULANCE",
    "category": "travel"
  },
  {
    "name": "fire_engine",
    "unicode": {"apple":"1F692", "google":"1F692", "twitter":"1F692"},
    "shortcode": "fire_engine",
    "description": "FIRE ENGINE",
    "category": "travel"
  },
  {
    "name": "minibus",
    "unicode": {"apple":"1F690", "google":"1F690", "twitter":"1F690"},
    "shortcode": "minibus",
    "description": "MINIBUS",
    "category": "travel"
  },
  {
    "name": "truck",
    "unicode": {"apple":"1F69A", "google":"1F69A", "twitter":"1F69A"},
    "shortcode": "truck",
    "description": "DELIVERY TRUCK",
    "category": "travel"
  },
  {
    "name": "articulated_lorry",
    "unicode": {"apple":"1F69B", "google":"1F69B", "twitter":"1F69B"},
    "shortcode": "articulated_lorry",
    "description": "ARTICULATED LORRY",
    "category": "travel"
  },
  {
    "name": "tractor",
    "unicode": {"apple":"1F69C", "google":"1F69C", "twitter":"1F69C"},
    "shortcode": "tractor",
    "description": "TRACTOR",
    "category": "travel"
  },
  {
    "name": "racing_motorcycle",
    "unicode": {"apple":"1F3CD", "google":"1F3CD", "twitter":"1F3CD"},
    "shortcode": "racing_motorcycle",
    "description": "Racing Motorcycle",
    "category": "travel"
  },
  {
    "name": "bike",
    "unicode": {"apple":"1F6B2", "google":"1F6B2", "twitter":"1F6B2"},
    "shortcode": "bike",
    "description": "BICYCLE",
    "category": "travel"
  },
  {
    "name": "rotating_light",
    "unicode": {"apple":"1F6A8", "google":"1F6A8", "twitter":"1F6A8"},
    "shortcode": "rotating_light",
    "description": "POLICE CARS REVOLVING LIGHT",
    "category": "travel"
  },
  {
    "name": "oncoming_police_car",
    "unicode": {"apple":"1F694", "google":"1F694", "twitter":"1F694"},
    "shortcode": "oncoming_police_car",
    "description": "ONCOMING POLICE CAR",
    "category": "travel"
  },
  {
    "name": "oncoming_bus",
    "unicode": {"apple":"1F68D", "google":"1F68D", "twitter":"1F68D"},
    "shortcode": "oncoming_bus",
    "description": "ONCOMING BUS",
    "category": "travel"
  },
  {
    "name": "oncoming_automobile",
    "unicode": {"apple":"1F698", "google":"1F698", "twitter":"1F698"},
    "shortcode": "oncoming_automobile",
    "description": "ONCOMING AUTOMOBILE",
    "category": "travel"
  },
  {
    "name": "oncoming_taxi",
    "unicode": {"apple":"1F696", "google":"1F696", "twitter":"1F696"},
    "shortcode": "oncoming_taxi",
    "description": "ONCOMING TAXI",
    "category": "travel"
  },
  {
    "name": "aerial_tramway",
    "unicode": {"apple":"1F6A1", "google":"1F6A1", "twitter":"1F6A1"},
    "shortcode": "aerial_tramway",
    "description": "AERIAL TRAMWAY",
    "category": "travel"
  },
  {
    "name": "mountain_cableway",
    "unicode": {"apple":"1F6A0", "google":"1F6A0", "twitter":"1F6A0"},
    "shortcode": "mountain_cableway",
    "description": "MOUNTAIN CABLEWAY",
    "category": "travel"
  },
  {
    "name": "suspension_railway",
    "unicode": {"apple":"1F69F", "google":"1F69F", "twitter":"1F69F"},
    "shortcode": "suspension_railway",
    "description": "SUSPENSION RAILWAY",
    "category": "travel"
  },
  {
    "name": "railway_car",
    "unicode": {"apple":"1F683", "google":"1F683", "twitter":"1F683"},
    "shortcode": "railway_car",
    "description": "RAILWAY CAR",
    "category": "travel"
  },
  {
    "name": "train",
    "unicode": {"apple":"1F68B", "google":"1F68B", "twitter":"1F68B"},
    "shortcode": "train",
    "description": "TRAM CAR",
    "category": "travel"
  },
  {
    "name": "monorail",
    "unicode": {"apple":"1F69D", "google":"1F69D", "twitter":"1F69D"},
    "shortcode": "monorail",
    "description": "MONORAIL",
    "category": "travel"
  },
  {
    "name": "bullettrain_side",
    "unicode": {"apple":"1F684", "google":"1F684", "twitter":"1F684"},
    "shortcode": "bullettrain_side",
    "description": "HIGH-SPEED TRAIN",
    "category": "travel"
  },
  {
    "name": "bullettrain_front",
    "unicode": {"apple":"1F685", "google":"1F685", "twitter":"1F685"},
    "shortcode": "bullettrain_front",
    "description": "HIGH-SPEED TRAIN WITH BULLET NOSE",
    "category": "travel"
  },
  {
    "name": "light_rail",
    "unicode": {"apple":"1F688", "google":"1F688", "twitter":"1F688"},
    "shortcode": "light_rail",
    "description": "LIGHT RAIL",
    "category": "travel"
  },
  {
    "name": "mountain_railway",
    "unicode": {"apple":"1F69E", "google":"1F69E", "twitter":"1F69E"},
    "shortcode": "mountain_railway",
    "description": "MOUNTAIN RAILWAY",
    "category": "travel"
  },
  {
    "name": "steam_locomotive",
    "unicode": {"apple":"1F682", "google":"1F682", "twitter":"1F682"},
    "shortcode": "steam_locomotive",
    "description": "STEAM LOCOMOTIVE",
    "category": "travel"
  },
  {
    "name": "train2",
    "unicode": {"apple":"1F686", "google":"1F686", "twitter":"1F686"},
    "shortcode": "train2",
    "description": "TRAIN",
    "category": "travel"
  },
  {
    "name": "metro",
    "unicode": {"apple":"1F687", "google":"1F687", "twitter":"1F687"},
    "shortcode": "metro",
    "description": "METRO",
    "category": "travel"
  },
  {
    "name": "tram",
    "unicode": {"apple":"1F68A", "google":"1F68A", "twitter":"1F68A"},
    "shortcode": "tram",
    "description": "TRAM",
    "category": "travel"
  },
  {
    "name": "station",
    "unicode": {"apple":"1F689", "google":"1F689", "twitter":"1F689"},
    "shortcode": "station",
    "description": "STATION",
    "category": "travel"
  },
  {
    "name": "helicopter",
    "unicode": {"apple":"1F681", "google":"1F681", "twitter":"1F681"},
    "shortcode": "helicopter",
    "description": "HELICOPTER",
    "category": "travel"
  },
  {
    "name": "small_airplane",
    "unicode": {"apple":"1F6E9", "google":"1F6E9", "twitter":"1F6E9"},
    "shortcode": "small_airplane",
    "description": "Small Airplane",
    "category": "travel"
  },
  {
    "name": "airplane",
    "unicode": {"apple":"2708", "google":"2708", "twitter":"2708"},
    "shortcode": "airplane",
    "description": "AIRPLANE",
    "category": "travel"
  },
  {
    "name": "airplane_departure",
    "unicode": {"apple":"1F6EB", "google":"1F6EB", "twitter":"1F6EB"},
    "shortcode": "airplane_departure",
    "description": "Airplane Departure",
    "category": "travel"
  },
  {
    "name": "airplane_arriving",
    "unicode": {"apple":"1F6EC", "google":"1F6EC", "twitter":"1F6EC"},
    "shortcode": "airplane_arriving",
    "description": "Airplane Arriving",
    "category": "travel"
  },
  {
    "name": "boat",
    "unicode": {"apple":"26F5", "google":"26F5", "twitter":"26F5"},
    "shortcode": "boat",
    "description": "SAILBOAT",
    "category": "travel"
  },
  {
    "name": "motor_boat",
    "unicode": {"apple":"1F6E5", "google":"1F6E5", "twitter":"1F6E5"},
    "shortcode": "motor_boat",
    "description": "Motor Boat",
    "category": "travel"
  },
  {
    "name": "speedboat",
    "unicode": {"apple":"1F6A4", "google":"1F6A4", "twitter":"1F6A4"},
    "shortcode": "speedboat",
    "description": "SPEEDBOAT",
    "category": "travel"
  },
  {
    "name": "ferry",
    "unicode": {"apple":"26F4", "google":"26F4", "twitter":"26F4"},
    "shortcode": "ferry",
    "description": "Ferry",
    "category": "travel"
  },
  {
    "name": "passenger_ship",
    "unicode": {"apple":"1F6F3", "google":"1F6F3", "twitter":"1F6F3"},
    "shortcode": "passenger_ship",
    "description": "Passenger Ship",
    "category": "travel"
  },
  {
    "name": "rocket",
    "unicode": {"apple":"1F680", "google":"1F680", "twitter":"1F680"},
    "shortcode": "rocket",
    "description": "ROCKET",
    "category": "travel"
  },
  {
    "name": "satellite",
    "unicode": {"apple":"1F4E1", "google":"1F4E1", "twitter":"1F4E1"},
    "shortcode": "satellite",
    "description": "SATELLITE ANTENNA",
    "category": "travel"
  },
  {
    "name": "seat",
    "unicode": {"apple":"1F4BA", "google":"1F4BA", "twitter":"1F4BA"},
    "shortcode": "seat",
    "description": "SEAT",
    "category": "travel"
  },
  {
    "name": "anchor",
    "unicode": {"apple":"2693", "google":"2693", "twitter":"2693"},
    "shortcode": "anchor",
    "description": "ANCHOR",
    "category": "travel"
  },
  {
    "name": "construction",
    "unicode": {"apple":"1F6A7", "google":"1F6A7", "twitter":"1F6A7"},
    "shortcode": "construction",
    "description": "CONSTRUCTION SIGN",
    "category": "travel"
  },
  {
    "name": "fuelpump",
    "unicode": {"apple":"26FD", "google":"26FD", "twitter":"26FD"},
    "shortcode": "fuelpump",
    "description": "FUEL PUMP",
    "category": "travel"
  },
  {
    "name": "busstop",
    "unicode": {"apple":"1F68F", "google":"1F68F", "twitter":"1F68F"},
    "shortcode": "busstop",
    "description": "BUS STOP",
    "category": "travel"
  },
  {
    "name": "vertical_traffic_light",
    "unicode": {"apple":"1F6A6", "google":"1F6A6", "twitter":"1F6A6"},
    "shortcode": "vertical_traffic_light",
    "description": "VERTICAL TRAFFIC LIGHT",
    "category": "travel"
  },
  {
    "name": "traffic_light",
    "unicode": {"apple":"1F6A5", "google":"1F6A5", "twitter":"1F6A5"},
    "shortcode": "traffic_light",
    "description": "HORIZONTAL TRAFFIC LIGHT",
    "category": "travel"
  },
  {
    "name": "checkered_flag",
    "unicode": {"apple":"1F3C1", "google":"1F3C1", "twitter":"1F3C1"},
    "shortcode": "checkered_flag",
    "description": "CHEQUERED FLAG",
    "category": "travel"
  },
  {
    "name": "ship",
    "unicode": {"apple":"1F6A2", "google":"1F6A2", "twitter":"1F6A2"},
    "shortcode": "ship",
    "description": "SHIP",
    "category": "travel"
  },
  {
    "name": "ferris_wheel",
    "unicode": {"apple":"1F3A1", "google":"1F3A1", "twitter":"1F3A1"},
    "shortcode": "ferris_wheel",
    "description": "FERRIS WHEEL",
    "category": "travel"
  },
  {
    "name": "roller_coaster",
    "unicode": {"apple":"1F3A2", "google":"1F3A2", "twitter":"1F3A2"},
    "shortcode": "roller_coaster",
    "description": "ROLLER COASTER",
    "category": "travel"
  },
  {
    "name": "carousel_horse",
    "unicode": {"apple":"1F3A0", "google":"1F3A0", "twitter":"1F3A0"},
    "shortcode": "carousel_horse",
    "description": "CAROUSEL HORSE",
    "category": "travel"
  },
  {
    "name": "building_construction",
    "unicode": {"apple":"1F3D7", "google":"1F3D7", "twitter":"1F3D7"},
    "shortcode": "building_construction",
    "description": "Building Construction",
    "category": "travel"
  },
  {
    "name": "foggy",
    "unicode": {"apple":"1F301", "google":"1F301", "twitter":"1F301"},
    "shortcode": "foggy",
    "description": "FOGGY",
    "category": "travel"
  },
  {
    "name": "tokyo_tower",
    "unicode": {"apple":"1F5FC", "google":"1F5FC", "twitter":"1F5FC"},
    "shortcode": "tokyo_tower",
    "description": "TOKYO TOWER",
    "category": "travel"
  },
  {
    "name": "factory",
    "unicode": {"apple":"1F3ED", "google":"1F3ED", "twitter":"1F3ED"},
    "shortcode": "factory",
    "description": "FACTORY",
    "category": "travel"
  },
  {
    "name": "fountain",
    "unicode": {"apple":"26F2", "google":"26F2", "twitter":"26F2"},
    "shortcode": "fountain",
    "description": "FOUNTAIN",
    "category": "travel"
  },
  {
    "name": "rice_scene",
    "unicode": {"apple":"1F391", "google":"1F391", "twitter":"1F391"},
    "shortcode": "rice_scene",
    "description": "MOON VIEWING CEREMONY",
    "category": "travel"
  },
  {
    "name": "mountain",
    "unicode": {"apple":"26F0", "google":"26F0", "twitter":"26F0"},
    "shortcode": "mountain",
    "description": "Mountain",
    "category": "travel"
  },
  {
    "name": "snow_capped_mountain",
    "unicode": {"apple":"1F3D4", "google":"1F3D4", "twitter":"1F3D4"},
    "shortcode": "snow_capped_mountain",
    "description": "Snow Capped Mountain",
    "category": "travel"
  },
  {
    "name": "mount_fuji",
    "unicode": {"apple":"1F5FB", "google":"1F5FB", "twitter":"1F5FB"},
    "shortcode": "mount_fuji",
    "description": "MOUNT FUJI",
    "category": "travel"
  },
  {
    "name": "volcano",
    "unicode": {"apple":"1F30B", "google":"1F30B", "twitter":"1F30B"},
    "shortcode": "volcano",
    "description": "VOLCANO",
    "category": "travel"
  },
  {
    "name": "japan",
    "unicode": {"apple":"1F5FE", "google":"1F5FE", "twitter":"1F5FE"},
    "shortcode": "japan",
    "description": "SILHOUETTE OF JAPAN",
    "category": "travel"
  },
  {
    "name": "camping",
    "unicode": {"apple":"1F3D5", "google":"1F3D5", "twitter":"1F3D5"},
    "shortcode": "camping",
    "description": "Camping",
    "category": "travel"
  },
  {
    "name": "tent",
    "unicode": {"apple":"26FA", "google":"26FA", "twitter":"26FA"},
    "shortcode": "tent",
    "description": "TENT",
    "category": "travel"
  },
  {
    "name": "national_park",
    "unicode": {"apple":"1F3DE", "google":"1F3DE", "twitter":"1F3DE"},
    "shortcode": "national_park",
    "description": "National Park",
    "category": "travel"
  },
  {
    "name": "motorway",
    "unicode": {"apple":"1F6E3", "google":"1F6E3", "twitter":"1F6E3"},
    "shortcode": "motorway",
    "description": "Motorway",
    "category": "travel"
  },
  {
    "name": "railway_track",
    "unicode": {"apple":"1F6E4", "google":"1F6E4", "twitter":"1F6E4"},
    "shortcode": "railway_track",
    "description": "Railway Track",
    "category": "travel"
  },
  {
    "name": "sunrise",
    "unicode": {"apple":"1F305", "google":"1F305", "twitter":"1F305"},
    "shortcode": "sunrise",
    "description": "SUNRISE",
    "category": "travel"
  },
  {
    "name": "sunrise_over_mountains",
    "unicode": {"apple":"1F304", "google":"1F304", "twitter":"1F304"},
    "shortcode": "sunrise_over_mountains",
    "description": "SUNRISE OVER MOUNTAINS",
    "category": "travel"
  },
  {
    "name": "desert",
    "unicode": {"apple":"1F3DC", "google":"1F3DC", "twitter":"1F3DC"},
    "shortcode": "desert",
    "description": "Desert",
    "category": "travel"
  },
  {
    "name": "beach_with_umbrella",
    "unicode": {"apple":"1F3D6", "google":"1F3D6", "twitter":"1F3D6"},
    "shortcode": "beach_with_umbrella",
    "description": "Beach With Umbrella",
    "category": "travel"
  },
  {
    "name": "desert_island",
    "unicode": {"apple":"1F3DD", "google":"1F3DD", "twitter":"1F3DD"},
    "shortcode": "desert_island",
    "description": "Desert Island",
    "category": "travel"
  },
  {
    "name": "city_sunrise",
    "unicode": {"apple":"1F307", "google":"1F307", "twitter":"1F307"},
    "shortcode": "city_sunrise",
    "description": "SUNSET OVER BUILDINGS",
    "category": "travel"
  },
  {
    "name": "city_sunset",
    "unicode": {"apple":"1F306", "google":"1F306", "twitter":"1F306"},
    "shortcode": "city_sunset",
    "description": "CITYSCAPE AT DUSK",
    "category": "travel"
  },
  {
    "name": "cityscape",
    "unicode": {"apple":"1F3D9", "google":"1F3D9", "twitter":"1F3D9"},
    "shortcode": "cityscape",
    "description": "CITYSCAPE",
    "category": "travel"
  },
  {
    "name": "night_with_stars",
    "unicode": {"apple":"1F303", "google":"1F303", "twitter":"1F303"},
    "shortcode": "night_with_stars",
    "description": "NIGHT WITH STARS",
    "category": "travel"
  },
  {
    "name": "bridge_at_night",
    "unicode": {"apple":"1F309", "google":"1F309", "twitter":"1F309"},
    "shortcode": "bridge_at_night",
    "description": "BRIDGE AT NIGHT",
    "category": "travel"
  },
  {
    "name": "milky_way",
    "unicode": {"apple":"1F30C", "google":"1F30C", "twitter":"1F30C"},
    "shortcode": "milky_way",
    "description": "MILKY WAY",
    "category": "travel"
  },
  {
    "name": "stars",
    "unicode": {"apple":"1F320", "google":"1F320", "twitter":"1F320"},
    "shortcode": "stars",
    "description": "SHOOTING STAR",
    "category": "travel"
  },
  {
    "name": "sparkler",
    "unicode": {"apple":"1F387", "google":"1F387", "twitter":"1F387"},
    "shortcode": "sparkler",
    "description": "FIREWORK SPARKLER",
    "category": "travel"
  },
  {
    "name": "fireworks",
    "unicode": {"apple":"1F386", "google":"1F386", "twitter":"1F386"},
    "shortcode": "fireworks",
    "description": "FIREWORKS",
    "category": "travel"
  },
  {
    "name": "rainbow",
    "unicode": {"apple":"1F308", "google":"1F308", "twitter":"1F308"},
    "shortcode": "rainbow",
    "description": "RAINBOW",
    "category": "travel"
  },
  {
    "name": "house_buildings",
    "unicode": {"apple":"1F3D8", "google":"1F3D8", "twitter":"1F3D8"},
    "shortcode": "house_buildings",
    "description": "EUROPEAN CASTLE",
    "category": "travel"
  },
  {
    "name": "european_castle",
    "unicode": {"apple":"1F3F0", "google":"1F3F0", "twitter":"1F3F0"},
    "shortcode": "european_castle",
    "description": "EUROPEAN CASTLE",
    "category": "travel"
  },
  {
    "name": "japanese_castle",
    "unicode": {"apple":"1F3EF", "google":"1F3EF", "twitter":"1F3EF"},
    "shortcode": "japanese_castle",
    "description": "JAPANESE CASTLE",
    "category": "travel"
  },
  {
    "name": "stadium",
    "unicode": {"apple":"1F3DF", "google":"1F3DF", "twitter":"1F3DF"},
    "shortcode": "stadium",
    "description": "Stadium",
    "category": "travel"
  },
  {
    "name": "statue_of_liberty",
    "unicode": {"apple":"1F5FD", "google":"1F5FD", "twitter":"1F5FD"},
    "shortcode": "statue_of_liberty",
    "description": "STATUE OF LIBERTY",
    "category": "travel"
  },
  {
    "name": "house",
    "unicode": {"apple":"1F3E0", "google":"1F3E0", "twitter":"1F3E0"},
    "shortcode": "house",
    "description": "HOUSE BUILDING",
    "category": "travel"
  },
  {
    "name": "house_with_garden",
    "unicode": {"apple":"1F3E1", "google":"1F3E1", "twitter":"1F3E1"},
    "shortcode": "house_with_garden",
    "description": "HOUSE WITH GARDEN",
    "category": "travel"
  },
  {
    "name": "derelict_house_building",
    "unicode": {"apple":"1F3DA", "google":"1F3DA", "twitter":"1F3DA"},
    "shortcode": "derelict_house_building",
    "description": "Derelict House Building",
    "category": "travel"
  },
  {
    "name": "office",
    "unicode": {"apple":"1F3E2", "google":"1F3E2", "twitter":"1F3E2"},
    "shortcode": "office",
    "description": "OFFICE BUILDING",
    "category": "travel"
  },
  {
    "name": "department_store",
    "unicode": {"apple":"1F3EC", "google":"1F3EC", "twitter":"1F3EC"},
    "shortcode": "department_store",
    "description": "DEPARTMENT STORE",
    "category": "travel"
  },
  {
    "name": "post_office",
    "unicode": {"apple":"1F3E3", "google":"1F3E3", "twitter":"1F3E3"},
    "shortcode": "post_office",
    "description": "JAPANESE POST OFFICE",
    "category": "travel"
  },
  {
    "name": "european_post_office",
    "unicode": {"apple":"1F3E4", "google":"1F3E4", "twitter":"1F3E4"},
    "shortcode": "european_post_office",
    "description": "EUROPEAN POST OFFICE",
    "category": "travel"
  },
  {
    "name": "hospital",
    "unicode": {"apple":"1F3E5", "google":"1F3E5", "twitter":"1F3E5"},
    "shortcode": "hospital",
    "description": "HOSPITAL",
    "category": "travel"
  },
  {
    "name": "bank",
    "unicode": {"apple":"1F3E6", "google":"1F3E6", "twitter":"1F3E6"},
    "shortcode": "bank",
    "description": "BANK",
    "category": "travel"
  },
  {
    "name": "hotel",
    "unicode": {"apple":"1F3E8", "google":"1F3E8", "twitter":"1F3E8"},
    "shortcode": "hotel",
    "description": "HOTEL",
    "category": "travel"
  },
  {
    "name": "convenience_store",
    "unicode": {"apple":"1F3EA", "google":"1F3EA", "twitter":"1F3EA"},
    "shortcode": "convenience_store",
    "description": "CONVENIENCE STORE",
    "category": "travel"
  },
  {
    "name": "school",
    "unicode": {"apple":"1F3EB", "google":"1F3EB", "twitter":"1F3EB"},
    "shortcode": "school",
    "description": "SCHOOL",
    "category": "travel"
  },
  {
    "name": "love_hotel",
    "unicode": {"apple":"1F3E9", "google":"1F3E9", "twitter":"1F3E9"},
    "shortcode": "love_hotel",
    "description": "LOVE HOTEL",
    "category": "travel"
  },
  {
    "name": "wedding",
    "unicode": {"apple":"1F492", "google":"1F492", "twitter":"1F492"},
    "shortcode": "wedding",
    "description": "WEDDING",
    "category": "travel"
  },
  {
    "name": "classical_building",
    "unicode": {"apple":"1F3DB", "google":"1F3DB", "twitter":"1F3DB"},
    "shortcode": "classical_building",
    "description": "Classical Building",
    "category": "travel"
  },
  {
    "name": "church",
    "unicode": {"apple":"26EA", "google":"26EA", "twitter":"26EA"},
    "shortcode": "church",
    "description": "CHURCH",
    "category": "travel"
  },
  {
    "name": "mosque",
    "unicode": {"apple":"1F54C", "google":"1F54C", "twitter":"1F54C"},
    "shortcode": "mosque",
    "description": "Mosque",
    "category": "travel"
  },
  {
    "name": "synagogue",
    "unicode": {"apple":"1F54D", "google":"1F54D", "twitter":"1F54D"},
    "shortcode": "synagogue",
    "description": "Synagogue",
    "category": "travel"
  },
  {
    "name": "kaaba",
    "unicode": {"apple":"1F54B", "google":"1F54B", "twitter":"1F54B"},
    "shortcode": "kaaba",
    "description": "Kaaba",
    "category": "travel"
  },
  {
    "name": "shinto_shrine",
    "unicode": {"apple":"26E9", "google":"26E9", "twitter":"26E9"},
    "shortcode": "shinto_shrine",
    "description": "Shinto Shrine",
    "category": "travel"
  },
  {
    "name": "watch",
    "unicode": {"apple":"231A", "google":"231A", "twitter":"231A"},
    "shortcode": "watch",
    "description": "WATCH",
    "category": "object"
  },
  {
    "name": "iphone",
    "unicode": {"apple":"1F4F1", "google":"1F4F1", "twitter":"1F4F1"},
    "shortcode": "iphone",
    "description": "MOBILE PHONE",
    "category": "object"
  },
  {
    "name": "calling",
    "unicode": {"apple":"1F4F2", "google":"1F4F2", "twitter":"1F4F2"},
    "shortcode": "calling",
    "description": "MOBILE PHONE WITH RIGHTWARDS ARROW AT LEFT",
    "category": "object"
  },
  {
    "name": "computer",
    "unicode": {"apple":"1F4BB", "google":"1F4BB", "twitter":"1F4BB"},
    "shortcode": "computer",
    "description": "PERSONAL COMPUTER",
    "category": "object"
  },
  {
    "name": "keyboard",
    "unicode": {"apple":"2328", "google":"2328", "twitter":"2328"},
    "shortcode": "keyboard",
    "description": "KEYBOARD",
    "category": "object"
  },
  {
    "name": "desktop_computer",
    "unicode": {"apple":"1F5A5", "google":"1F5A5", "twitter":"1F5A5"},
    "shortcode": "desktop_computer",
    "description": "DESKTOP COMPUTER",
    "category": "object"
  },
  {
    "name": "printer",
    "unicode": {"apple":"1F5A8", "google":"1F5A8", "twitter":"1F5A8"},
    "shortcode": "printer",
    "description": "PRINTER",
    "category": "object"
  },
  {
    "name": "three_button_mouse",
    "unicode": {"apple":"1F5B1", "google":"1F5B1", "twitter":"1F5B1"},
    "shortcode": "three_button_mouse",
    "description": "THREE BUTTON MOUSE",
    "category": "object"
  },
  {
    "name": "trackball",
    "unicode": {"apple":"1F5B2", "google":"1F5B2", "twitter":"1F5B2"},
    "shortcode": "trackball",
    "description": "TRACKBALL",
    "category": "object"
  },
  {
    "name": "joystick",
    "unicode": {"apple":"1F579", "google":"1F579", "twitter":"1F579"},
    "shortcode": "joystick",
    "description": "JOYSTICK",
    "category": "object"
  },
  {
    "name": "compression",
    "unicode": {"apple":"1F5DC", "google":"1F5DC", "twitter":"1F5DC"},
    "shortcode": "compression",
    "description": "COMPRESSION",
    "category": "object"
  },
  {
    "name": "minidisc",
    "unicode": {"apple":"1F4BD", "google":"1F4BD", "twitter":"1F4BD"},
    "shortcode": "minidisc",
    "description": "MINIDISC",
    "category": "object"
  },
  {
    "name": "floppy_disk",
    "unicode": {"apple":"1F4BE", "google":"1F4BE", "twitter":"1F4BE"},
    "shortcode": "floppy_disk",
    "description": "FLOPPY DISK",
    "category": "object"
  },
  {
    "name": "cd",
    "unicode": {"apple":"1F4BF", "google":"1F4BF", "twitter":"1F4BF"},
    "shortcode": "cd",
    "description": "OPTICAL DISC",
    "category": "object"
  },
  {
    "name": "dvd",
    "unicode": {"apple":"1F4C0", "google":"1F4C0", "twitter":"1F4C0"},
    "shortcode": "dvd",
    "description": "DVD",
    "category": "object"
  },
  {
    "name": "vhs",
    "unicode": {"apple":"1F4FC", "google":"1F4FC", "twitter":"1F4FC"},
    "shortcode": "vhs",
    "description": "VIDEOCASSETTE",
    "category": "object"
  },
  {
    "name": "camera",
    "unicode": {"apple":"1F4F7", "google":"1F4F7", "twitter":"1F4F7"},
    "shortcode": "camera",
    "description": "CAMERA",
    "category": "object"
  },
  {
    "name": "camera_with_flash",
    "unicode": {"apple":"1F4F8", "google":"1F4F8", "twitter":"1F4F8"},
    "shortcode": "camera_with_flash",
    "description": "CAMERA WITH FLASH",
    "category": "object"
  },
  {
    "name": "video_camera",
    "unicode": {"apple":"1F4F9", "google":"1F4F9", "twitter":"1F4F9"},
    "shortcode": "video_camera",
    "description": "VIDEO CAMERA",
    "category": "object"
  },
  {
    "name": "movie_camera",
    "unicode": {"apple":"1F3A5", "google":"1F3A5", "twitter":"1F3A5"},
    "shortcode": "movie_camera",
    "description": "MOVIE CAMERA",
    "category": "object"
  },
  {
    "name": "film_projector",
    "unicode": {"apple":"1F4FD", "google":"1F4FD", "twitter":"1F4FD"},
    "shortcode": "film_projector",
    "description": "FILM PROJECTOR",
    "category": "object"
  },
  {
    "name": "film_frames",
    "unicode": {"apple":"1F39E", "google":"1F39E", "twitter":"1F39E"},
    "shortcode": "film_frames",
    "description": "FILM FRAMES",
    "category": "object"
  },
  {
    "name": "telephone_receiver",
    "unicode": {"apple":"1F4DE", "google":"1F4DE", "twitter":"1F4DE"},
    "shortcode": "telephone_receiver",
    "description": "TELEPHONE RECEIVER",
    "category": "object"
  },
  {
    "name": "phone",
    "unicode": {"apple":"260E", "google":"260E", "twitter":"260E"},
    "shortcode": "phone",
    "description": "BLACK TELEPHONE",
    "category": "object"
  },
  {
    "name": "pager",
    "unicode": {"apple":"1F4DF", "google":"1F4DF", "twitter":"1F4DF"},
    "shortcode": "pager",
    "description": "PAGER",
    "category": "object"
  },
  {
    "name": "fax",
    "unicode": {"apple":"1F4E0", "google":"1F4E0", "twitter":"1F4E0"},
    "shortcode": "fax",
    "description": "FAX MACHINE",
    "category": "object"
  },
  {
    "name": "tv",
    "unicode": {"apple":"1F4FA", "google":"1F4FA", "twitter":"1F4FA"},
    "shortcode": "tv",
    "description": "TELEVISION",
    "category": "object"
  },
  {
    "name": "radio",
    "unicode": {"apple":"1F4FB", "google":"1F4FB", "twitter":"1F4FB"},
    "shortcode": "radio",
    "description": "RADIO",
    "category": "object"
  },
  {
    "name": "studio_microphone",
    "unicode": {"apple":"1F399", "google":"1F399", "twitter":"1F399"},
    "shortcode": "studio_microphone",
    "description": "STUDIO MICROPHONE",
    "category": "object"
  },
  {
    "name": "level_slider",
    "unicode": {"apple":"1F39A", "google":"1F39A", "twitter":"1F39A"},
    "shortcode": "level_slider",
    "description": "LEVEL SLIDER",
    "category": "object"
  },
  {
    "name": "control_knobs",
    "unicode": {"apple":"1F39B", "google":"1F39B", "twitter":"1F39B"},
    "shortcode": "control_knobs",
    "description": "CONTROL KNOBS",
    "category": "object"
  },
  {
    "name": "stopwatch",
    "unicode": {"apple":"23F1", "google":"23F1", "twitter":"23F1"},
    "shortcode": "stopwatch",
    "description": "STOPWATCH",
    "category": "object"
  },
  {
    "name": "timer_clock",
    "unicode": {"apple":"23F2", "google":"23F2", "twitter":"23F2"},
    "shortcode": "timer_clock",
    "description": "TIMER CLOCK",
    "category": "object"
  },
  {
    "name": "alarm_clock",
    "unicode": {"apple":"23F0", "google":"23F0", "twitter":"23F0"},
    "shortcode": "alarm_clock",
    "description": "ALARM CLOCK",
    "category": "object"
  },
  {
    "name": "mantelpiece_clock",
    "unicode": {"apple":"1F570", "google":"1F570", "twitter":"1F570"},
    "shortcode": "mantelpiece_clock",
    "description": "MANTELPIECE CLOCK",
    "category": "object"
  },
  {
    "name": "hourglass_flowing_sand",
    "unicode": {"apple":"23F3", "google":"23F3", "twitter":"23F3"},
    "shortcode": "hourglass_flowing_sand",
    "description": "HOURGLASS WITH FLOWING SAND",
    "category": "object"
  },
  {
    "name": "hourglass",
    "unicode": {"apple":"231B", "google":"231B", "twitter":"231B"},
    "shortcode": "hourglass",
    "description": "HOURGLASS",
    "category": "object"
  },
  {
    "name": "satellite",
    "unicode": {"apple":"1F6F0", "google":"1F6F0", "twitter":"1F6F0"},
    "shortcode": "satellite",
    "description": "SATELLITE",
    "category": "object"
  },
  {
    "name": "battery",
    "unicode": {"apple":"1F50B", "google":"1F50B", "twitter":"1F50B"},
    "shortcode": "battery",
    "description": "BATTERY",
    "category": "object"
  },
  {
    "name": "electric_plug",
    "unicode": {"apple":"1F50C", "google":"1F50C", "twitter":"1F50C"},
    "shortcode": "electric_plug",
    "description": "ELECTRIC PLUG",
    "category": "object"
  },
  {
    "name": "bulb",
    "unicode": {"apple":"1F4A1", "google":"1F4A1", "twitter":"1F4A1"},
    "shortcode": "bulb",
    "description": "ELECTRIC LIGHT BULB",
    "category": "object"
  },
  {
    "name": "flashlight",
    "unicode": {"apple":"1F526", "google":"1F526", "twitter":"1F526"},
    "shortcode": "flashlight",
    "description": "ELECTRIC TORCH",
    "category": "object"
  },
  {
    "name": "candle",
    "unicode": {"apple":"1F56F", "google":"1F56F", "twitter":"1F56F"},
    "shortcode": "candle",
    "description": "CANDLE",
    "category": "object"
  },
  {
    "name": "wastebasket",
    "unicode": {"apple":"1F5D1", "google":"1F5D1", "twitter":"1F5D1"},
    "shortcode": "wastebasket",
    "description": "WASTEBASKET",
    "category": "object"
  },
  {
    "name": "oil_drum",
    "unicode": {"apple":"1F6E2", "google":"1F6E2", "twitter":"1F6E2"},
    "shortcode": "oil_drum",
    "description": "OIL DRUM",
    "category": "object"
  },
  {
    "name": "money_with_wings",
    "unicode": {"apple":"1F4B8", "google":"1F4B8", "twitter":"1F4B8"},
    "shortcode": "money_with_wings",
    "description": "MONEY WITH WINGS",
    "category": "object"
  },
  {
    "name": "dollar",
    "unicode": {"apple":"1F4B5", "google":"1F4B5", "twitter":"1F4B5"},
    "shortcode": "dollar",
    "description": "BANKNOTE WITH DOLLAR SIGN",
    "category": "object"
  },
  {
    "name": "yen",
    "unicode": {"apple":"1F4B4", "google":"1F4B4", "twitter":"1F4B4"},
    "shortcode": "yen",
    "description": "BANKNOTE WITH YEN SIGN",
    "category": "object"
  },
  {
    "name": "euro",
    "unicode": {"apple":"1F4B6", "google":"1F4B6", "twitter":"1F4B6"},
    "shortcode": "euro",
    "description": "BANKNOTE WITH EURO SIGN",
    "category": "object"
  },
  {
    "name": "pound",
    "unicode": {"apple":"1F4B7", "google":"1F4B7", "twitter":"1F4B7"},
    "shortcode": "pound",
    "description": "BANKNOTE WITH POUND SIGN",
    "category": "object"
  },
  {
    "name": "moneybag",
    "unicode": {"apple":"1F4B0", "google":"1F4B0", "twitter":"1F4B0"},
    "shortcode": "moneybag",
    "description": "MONEY BAG",
    "category": "object"
  },
  {
    "name": "credit_card",
    "unicode": {"apple":"1F4B3", "google":"1F4B3", "twitter":"1F4B3"},
    "shortcode": "credit_card",
    "description": "CREDIT CARD",
    "category": "object"
  },
  {
    "name": "gem",
    "unicode": {"apple":"1F48E", "google":"1F48E", "twitter":"1F48E"},
    "shortcode": "gem",
    "description": "GEM STONE",
    "category": "object"
  },
  {
    "name": "scales",
    "unicode": {"apple":"2696", "google":"2696", "twitter":"2696"},
    "shortcode": "scales",
    "description": "SCALES",
    "category": "object"
  },
  {
    "name": "wrench",
    "unicode": {"apple":"1F527", "google":"1F527", "twitter":"1F527"},
    "shortcode": "wrench",
    "description": "WRENCH",
    "category": "object"
  },
  {
    "name": "hammer",
    "unicode": {"apple":"1F528", "google":"1F528", "twitter":"1F528"},
    "shortcode": "hammer",
    "description": "HAMMER",
    "category": "object"
  },
  {
    "name": "hammer_and_pick",
    "unicode": {"apple":"2692", "google":"2692", "twitter":"2692"},
    "shortcode": "hammer_and_pick",
    "description": "HAMMER AND PICK",
    "category": "object"
  },
  {
    "name": "hammer_and_wrench",
    "unicode": {"apple":"1F6E0", "google":"1F6E0", "twitter":"1F6E0"},
    "shortcode": "hammer_and_wrench",
    "description": "HAMMER AND WRENCH",
    "category": "object"
  },
  {
    "name": "pick",
    "unicode": {"apple":"26CF", "google":"26CF", "twitter":"26CF"},
    "shortcode": "pick",
    "description": "PICK",
    "category": "object"
  },
  {
    "name": "nut_and_bolt",
    "unicode": {"apple":"1F529", "google":"1F529", "twitter":"1F529"},
    "shortcode": "nut_and_bolt",
    "description": "NUT AND BOLT",
    "category": "object"
  },
  {
    "name": "gear",
    "unicode": {"apple":"2699", "google":"2699", "twitter":"2699"},
    "shortcode": "gear",
    "description": "GEAR",
    "category": "object"
  },
  {
    "name": "chains",
    "unicode": {"apple":"26D3", "google":"26D3", "twitter":"26D3"},
    "shortcode": "chains",
    "description": "CHAINS",
    "category": "object"
  },
  {
    "name": "gun",
    "unicode": {"apple":"1F52B", "google":"1F52B", "twitter":"1F52B"},
    "shortcode": "gun",
    "description": "PISTOL",
    "category": "object"
  },
  {
    "name": "bomb",
    "unicode": {"apple":"1F4A3", "google":"1F4A3", "twitter":"1F4A3"},
    "shortcode": "bomb",
    "description": "BOMB",
    "category": "object"
  },
  {
    "name": "hocho",
    "unicode": {"apple":"1F52A", "google":"1F52A", "twitter":"1F52A"},
    "shortcode": "hocho",
    "description": "HOCHO",
    "category": "object"
  },
  {
    "name": "dagger_knife",
    "unicode": {"apple":"1F5E1", "google":"1F5E1", "twitter":"1F5E1"},
    "shortcode": "dagger_knife",
    "description": "DAGGER KNIFE",
    "category": "object"
  },
  {
    "name": "crossed_swords",
    "unicode": {"apple":"2694", "google":"2694", "twitter":"2694"},
    "shortcode": "crossed_swords",
    "description": "CROSSED SWORDS",
    "category": "object"
  },
  {
    "name": "shield",
    "unicode": {"apple":"1F6E1", "google":"1F6E1", "twitter":"1F6E1"},
    "shortcode": "shield",
    "description": "SHIELD",
    "category": "object"
  },
  {
    "name": "smoking",
    "unicode": {"apple":"1F6AC", "google":"1F6AC", "twitter":"1F6AC"},
    "shortcode": "smoking",
    "description": "SMOKING SYMBOL",
    "category": "object"
  },
  {
    "name": "skull_and_crossbones",
    "unicode": {"apple":"2620", "google":"2620", "twitter":"2620"},
    "shortcode": "skull_and_crossbones",
    "description": "SKULL AND CROSSBONES",
    "category": "object"
  },
  {
    "name": "coffin",
    "unicode": {"apple":"26B0", "google":"26B0", "twitter":"26B0"},
    "shortcode": "coffin",
    "description": "COFFIN",
    "category": "object"
  },
  {
    "name": "funeral_urn",
    "unicode": {"apple":"26B1", "google":"26B1", "twitter":"26B1"},
    "shortcode": "funeral_urn",
    "description": "FUNERAL URN",
    "category": "object"
  },
  {
    "name": "amphora",
    "unicode": {"apple":"1F3FA", "google":"1F3FA", "twitter":"1F3FA"},
    "shortcode": "amphora",
    "description": "AMPHORA",
    "category": "object"
  },
  {
    "name": "crystal_ball",
    "unicode": {"apple":"1F52E", "google":"1F52E", "twitter":"1F52E"},
    "shortcode": "crystal_ball",
    "description": "CRYSTAL BALL",
    "category": "object"
  },
  {
    "name": "prayer_beads",
    "unicode": {"apple":"1F4FF", "google":"1F4FF", "twitter":"1F4FF"},
    "shortcode": "prayer_beads",
    "description": "PRAYER BEADS",
    "category": "object"
  },
  {
    "name": "barber",
    "unicode": {"apple":"1F488", "google":"1F488", "twitter":"1F488"},
    "shortcode": "barber",
    "description": "BARBER POLE",
    "category": "object"
  },
  {
    "name": "alembic",
    "unicode": {"apple":"2697", "google":"2697", "twitter":"2697"},
    "shortcode": "alembic",
    "description": "ALEMBIC",
    "category": "object"
  },
  {
    "name": "telescope",
    "unicode": {"apple":"1F52D", "google":"1F52D", "twitter":"1F52D"},
    "shortcode": "telescope",
    "description": "TELESCOPE",
    "category": "object"
  },
  {
    "name": "microscope",
    "unicode": {"apple":"1F52C", "google":"1F52C", "twitter":"1F52C"},
    "shortcode": "microscope",
    "description": "MICROSCOPE",
    "category": "object"
  },
  {
    "name": "hole",
    "unicode": {"apple":"1F573", "google":"1F573", "twitter":"1F573"},
    "shortcode": "hole",
    "description": "HOLE",
    "category": "object"
  },
  {
    "name": "pill",
    "unicode": {"apple":"1F48A", "google":"1F48A", "twitter":"1F48A"},
    "shortcode": "pill",
    "description": "PILL",
    "category": "object"
  },
  {
    "name": "syringe",
    "unicode": {"apple":"1F489", "google":"1F489", "twitter":"1F489"},
    "shortcode": "syringe",
    "description": "SYRINGE",
    "category": "object"
  },
  {
    "name": "thermometer",
    "unicode": {"apple":"1F321", "google":"1F321", "twitter":"1F321"},
    "shortcode": "thermometer",
    "description": "THERMOMETER",
    "category": "object"
  },
  {
    "name": "label",
    "unicode": {"apple":"1F3F7", "google":"1F3F7", "twitter":"1F3F7"},
    "shortcode": "label",
    "description": "LABEL",
    "category": "object"
  },
  {
    "name": "bookmark",
    "unicode": {"apple":"1F516", "google":"1F516", "twitter":"1F516"},
    "shortcode": "bookmark",
    "description": "BOOKMARK",
    "category": "object"
  },
  {
    "name": "toilet",
    "unicode": {"apple":"1F6BD", "google":"1F6BD", "twitter":"1F6BD"},
    "shortcode": "toilet",
    "description": "TOILET",
    "category": "object"
  },
  {
    "name": "shower",
    "unicode": {"apple":"1F6BF", "google":"1F6BF", "twitter":"1F6BF"},
    "shortcode": "shower",
    "description": "SHOWER",
    "category": "object"
  },
  {
    "name": "bathtub",
    "unicode": {"apple":"1F6C1", "google":"1F6C1", "twitter":"1F6C1"},
    "shortcode": "bathtub",
    "description": "BATHTUB",
    "category": "object"
  },
  {
    "name": "key",
    "unicode": {"apple":"1F511", "google":"1F511", "twitter":"1F511"},
    "shortcode": "key",
    "description": "KEY",
    "category": "object"
  },
  {
    "name": "old_key",
    "unicode": {"apple":"1F5DD", "google":"1F5DD", "twitter":"1F5DD"},
    "shortcode": "old_key",
    "description": "OLD KEY",
    "category": "object"
  },
  {
    "name": "couch_and_lamp",
    "unicode": {"apple":"1F6CB", "google":"1F6CB", "twitter":"1F6CB"},
    "shortcode": "couch_and_lamp",
    "description": "COUCH AND LAMP",
    "category": "object"
  },
  {
    "name": "sleeping_accommodation",
    "unicode": {"apple":"1F6CC", "google":"1F6CC", "twitter":"1F6CC"},
    "shortcode": "sleeping_accommodation",
    "description": "SLEEPING ACCOMMODATION",
    "category": "object"
  },
  {
    "name": "bed",
    "unicode": {"apple":"1F6CF", "google":"1F6CF", "twitter":"1F6CF"},
    "shortcode": "bed",
    "description": "BED",
    "category": "object"
  },
  {
    "name": "door",
    "unicode": {"apple":"1F6AA", "google":"1F6AA", "twitter":"1F6AA"},
    "shortcode": "door",
    "description": "DOOR",
    "category": "object"
  },
  {
    "name": "bellhop_bell",
    "unicode": {"apple":"1F6CE", "google":"1F6CE", "twitter":"1F6CE"},
    "shortcode": "bellhop_bell",
    "description": "BELLHOP BELL",
    "category": "object"
  },
  {
    "name": "frame_with_picture",
    "unicode": {"apple":"1F5BC", "google":"1F5BC", "twitter":"1F5BC"},
    "shortcode": "frame_with_picture",
    "description": "FRAME WITH PICTURE",
    "category": "object"
  },
  {
    "name": "world_map",
    "unicode": {"apple":"1F5FA", "google":"1F5FA", "twitter":"1F5FA"},
    "shortcode": "world_map",
    "description": "WORLD MAP",
    "category": "object"
  },
  {
    "name": "umbrella_on_ground",
    "unicode": {"apple":"26F1", "google":"26F1", "twitter":"26F1"},
    "shortcode": "umbrella_on_ground",
    "description": "UMBRELLA ON GROUND",
    "category": "object"
  },
  {
    "name": "moyai",
    "unicode": {"apple":"1F5FF", "google":"1F5FF", "twitter":"1F5FF"},
    "shortcode": "moyai",
    "description": "MOYAI",
    "category": "object"
  },
  {
    "name": "shopping_bags",
    "unicode": {"apple":"1F6CD", "google":"1F6CD", "twitter":"1F6CD"},
    "shortcode": "shopping_bags",
    "description": "SHOPPING BAGS",
    "category": "object"
  },
  {
    "name": "balloon",
    "unicode": {"apple":"1F388", "google":"1F388", "twitter":"1F388"},
    "shortcode": "balloon",
    "description": "BALLOON",
    "category": "object"
  },
  {
    "name": "flags",
    "unicode": {"apple":"1F38F", "google":"1F38F", "twitter":"1F38F"},
    "shortcode": "flags",
    "description": "CARP STREAMER",
    "category": "object"
  },
  {
    "name": "ribbon",
    "unicode": {"apple":"1F380", "google":"1F380", "twitter":"1F380"},
    "shortcode": "ribbon",
    "description": "RIBBON",
    "category": "object"
  },
  {
    "name": "gift",
    "unicode": {"apple":"1F381", "google":"1F381", "twitter":"1F381"},
    "shortcode": "gift",
    "description": "WRAPPED PRESENT",
    "category": "object"
  },
{
    "name": "confetti_ball",
    "unicode": {"apple":"1F38A", "google":"1F38A", "twitter":"1F38A"},
    "shortcode": "confetti_ball",
    "description": "CONFETTI BALL",
    "category": "object"
  },
  {
    "name": "tada",
    "unicode": {"apple":"1F389", "google":"1F389", "twitter":"1F389"},
    "shortcode": "tada",
    "description": "PARTY POPPER",
    "category": "object"
  },
  {
    "name": "dolls",
    "unicode": {"apple":"1F38E", "google":"1F38E", "twitter":"1F38E"},
    "shortcode": "dolls",
    "description": "JAPANESE DOLLS",
    "category": "object"
  },
  {
    "name": "wind_chime",
    "unicode": {"apple":"1F390", "google":"1F390", "twitter":"1F390"},
    "shortcode": "wind_chime",
    "description": "WIND CHIME",
    "category": "object"
  },
  {
    "name": "crossed_flags",
    "unicode": {"apple":"1F38C", "google":"1F38C", "twitter":"1F38C"},
    "shortcode": "crossed_flags",
    "description": "CROSSED FLAGS",
    "category": "object"
  },
  {
    "name": "izakaya_lantern",
    "unicode": {"apple":"1F3EE", "google":"1F3EE", "twitter":"1F3EE"},
    "shortcode": "izakaya_lantern",
    "description": "IZAKAYA LANTERN",
    "category": "object"
  },
  {
    "name": "email",
    "unicode": {"apple":"2709", "google":"2709", "twitter":"2709"},
    "shortcode": "email",
    "description": "ENVELOPE",
    "category": "object"
  },
  {
    "name": "envelope_with_arrow",
    "unicode": {"apple":"1F4E9", "google":"1F4E9", "twitter":"1F4E9"},
    "shortcode": "envelope_with_arrow",
    "description": "ENVELOPE WITH DOWNWARDS ARROW ABOVE",
    "category": "object"
  },
  {
    "name": "incoming_envelope",
    "unicode": {"apple":"1F4E8", "google":"1F4E8", "twitter":"1F4E8"},
    "shortcode": "incoming_envelope",
    "description": "INCOMING ENVELOPE",
    "category": "object"
  },
  {
    "name": "e-mail",
    "unicode": {"apple":"1F4E7", "google":"1F4E7", "twitter":"1F4E7"},
    "shortcode": "e-mail",
    "description": "E-MAIL SYMBOL",
    "category": "object"
  },
  {
    "name": "love_letter",
    "unicode": {"apple":"1F48C", "google":"1F48C", "twitter":"1F48C"},
    "shortcode": "love_letter",
    "description": "LOVE LETTER",
    "category": "object"
  },
  {
    "name": "postbox",
    "unicode": {"apple":"1F4EE", "google":"1F4EE", "twitter":"1F4EE"},
    "shortcode": "postbox",
    "description": "POSTBOX",
    "category": "object"
  },
  {
    "name": "mailbox_closed",
    "unicode": {"apple":"1F4EA", "google":"1F4EA", "twitter":"1F4EA"},
    "shortcode": "mailbox_closed",
    "description": "CLOSED MAILBOX WITH LOWERED FLAG",
    "category": "object"
  },
  {
    "name": "mailbox",
    "unicode": {"apple":"1F4EB", "google":"1F4EB", "twitter":"1F4EB"},
    "shortcode": "mailbox",
    "description": "CLOSED MAILBOX WITH RAISED FLAG",
    "category": "object"
  },
  {
    "name": "mailbox_with_mail",
    "unicode": {"apple":"1F4EC", "google":"1F4EC", "twitter":"1F4EC"},
    "shortcode": "mailbox_with_mail",
    "description": "OPEN MAILBOX WITH RAISED FLAG",
    "category": "object"
  },
  {
    "name": "mailbox_with_no_mail",
    "unicode": {"apple":"1F4ED", "google":"1F4ED", "twitter":"1F4ED"},
    "shortcode": "mailbox_with_no_mail",
    "description": "OPEN MAILBOX WITH LOWERED FLAG",
    "category": "object"
  },
  {
    "name": "package",
    "unicode": {"apple":"1F4E6", "google":"1F4E6", "twitter":"1F4E6"},
    "shortcode": "package",
    "description": "PACKAGE",
    "category": "object"
  },
  {
    "name": "postal_horn",
    "unicode": {"apple":"1F4EF", "google":"1F4EF", "twitter":"1F4EF"},
    "shortcode": "postal_horn",
    "description": "POSTAL HORN",
    "category": "object"
  },
  {
    "name": "inbox_tray",
    "unicode": {"apple":"1F4E5", "google":"1F4E5", "twitter":"1F4E5"},
    "shortcode": "inbox_tray",
    "description": "INBOX TRAY",
    "category": "object"
  },
  {
    "name": "outbox_tray",
    "unicode": {"apple":"1F4E4", "google":"1F4E4", "twitter":"1F4E4"},
    "shortcode": "outbox_tray",
    "description": "OUTBOX TRAY",
    "category": "object"
  },
  {
    "name": "scroll",
    "unicode": {"apple":"1F4DC", "google":"1F4DC", "twitter":"1F4DC"},
    "shortcode": "scroll",
    "description": "SCROLL",
    "category": "object"
  },
  {
    "name": "page_with_curl",
    "unicode": {"apple":"1F4C3", "google":"1F4C3", "twitter":"1F4C3"},
    "shortcode": "page_with_curl",
    "description": "PAGE WITH CURL",
    "category": "object"
  },
  {
    "name": "bookmark_tabs",
    "unicode": {"apple":"1F4D1", "google":"1F4D1", "twitter":"1F4D1"},
    "shortcode": "bookmark_tabs",
    "description": "BOOKMARK TABS",
    "category": "object"
  },
  {
    "name": "bar_chart",
    "unicode": {"apple":"1F4CA", "google":"1F4CA", "twitter":"1F4CA"},
    "shortcode": "bar_chart",
    "description": "BAR CHART",
    "category": "object"
  },
  {
    "name": "chart_with_upwards_trend",
    "unicode": {"apple":"1F4C8", "google":"1F4C8", "twitter":"1F4C8"},
    "shortcode": "chart_with_upwards_trend",
    "description": "CHART WITH UPWARDS TREND",
    "category": "object"
  },
  {
    "name": "chart_with_downwards_trend",
    "unicode": {"apple":"1F4C9", "google":"1F4C9", "twitter":"1F4C9"},
    "shortcode": "chart_with_downwards_trend",
    "description": "CHART WITH DOWNWARDS TREND",
    "category": "object"
  },
  {
    "name": "page_facing_up",
    "unicode": {"apple":"1F4C4", "google":"1F4C4", "twitter":"1F4C4"},
    "shortcode": "page_facing_up",
    "description": "PAGE FACING UP",
    "category": "object"
  },
  {
    "name": "date",
    "unicode": {"apple":"1F4C5", "google":"1F4C5", "twitter":"1F4C5"},
    "shortcode": "date",
    "description": "CALENDAR",
    "category": "object"
  },
  {
    "name": "calendar",
    "unicode": {"apple":"1F4C6", "google":"1F4C6", "twitter":"1F4C6"},
    "shortcode": "calendar",
    "description": "TEAR-OFF CALENDAR",
    "category": "object"
  },
  {
    "name": "spiral_calendar_pad",
    "unicode": {"apple":"1F5D3", "google":"1F5D3", "twitter":"1F5D3"},
    "shortcode": "spiral_calendar_pad",
    "description": "SPIRAL CALENDAR PAD",
    "category": "object"
  },
  {
    "name": "card_index",
    "unicode": {"apple":"1F4C7", "google":"1F4C7", "twitter":"1F4C7"},
    "shortcode": "card_index",
    "description": "CARD INDEX",
    "category": "object"
  },
  {
    "name": "card_file_box",
    "unicode": {"apple":"1F5C3", "google":"1F5C3", "twitter":"1F5C3"},
    "shortcode": "card_file_box",
    "description": "CARD FILE BOX",
    "category": "object"
  },
  {
    "name": "ballot_box_with_check",
    "unicode": {"apple":"2611-FE0F", "google":"2611-FE0F", "twitter":"2611-FE0F"},
    "shortcode": "ballot_box_with_check",
    "description": "BALLOT BOX WITH CHECK",
    "category": "object"
  },
  {
    "name": "file_cabinet",
    "unicode": {"apple":"1F5C4", "google":"1F5C4", "twitter":"1F5C4"},
    "shortcode": "file_cabinet",
    "description": "FILE CABINET",
    "category": "object"
  },
  {
    "name": "clipboard",
    "unicode": {"apple":"1F4CB", "google":"1F4CB", "twitter":"1F4CB"},
    "shortcode": "clipboard",
    "description": "CLIPBOARD",
    "category": "object"
  },
  {
    "name": "spiral_note_pad",
    "unicode": {"apple":"1F5D2", "google":"1F5D2", "twitter":"1F5D2"},
    "shortcode": "spiral_note_pad",
    "description": "SPIRAL NOTE PAD",
    "category": "object"
  },
  {
    "name": "file_folder",
    "unicode": {"apple":"1F4C1", "google":"1F4C1", "twitter":"1F4C1"},
    "shortcode": "file_folder",
    "description": "FILE FOLDER",
    "category": "object"
  },
  {
    "name": "open_file_folder",
    "unicode": {"apple":"1F4C2", "google":"1F4C2", "twitter":"1F4C2"},
    "shortcode": "open_file_folder",
    "description": "OPEN FILE FOLDER",
    "category": "object"
  },
  {
    "name": "card_index_dividers",
    "unicode": {"apple":"1F5C2", "google":"1F5C2", "twitter":"1F5C2"},
    "shortcode": "card_index_dividers",
    "description": "CARD INDEX DIVIDERS",
    "category": "object"
  },
  {
    "name": "rolled_up_newspaper",
    "unicode": {"apple":"1F5DE", "google":"1F5DE", "twitter":"1F5DE"},
    "shortcode": "rolled_up_newspaper",
    "description": "ROLLED-UP NEWSPAPER",
    "category": "object"
  },
  {
    "name": "newspaper",
    "unicode": {"apple":"1F4F0", "google":"1F4F0", "twitter":"1F4F0"},
    "shortcode": "newspaper",
    "description": "NEWSPAPER",
    "category": "object"
  },
  {
    "name": "notebook",
    "unicode": {"apple":"1F4D3", "google":"1F4D3", "twitter":"1F4D3"},
    "shortcode": "notebook",
    "description": "NOTEBOOK",
    "category": "object"
  },
  {
    "name": "closed_book",
    "unicode": {"apple":"1F4D5", "google":"1F4D5", "twitter":"1F4D5"},
    "shortcode": "closed_book",
    "description": "CLOSED BOOK",
    "category": "object"
  },
  {
    "name": "green_book",
    "unicode": {"apple":"1F4D7", "google":"1F4D7", "twitter":"1F4D7"},
    "shortcode": "green_book",
    "description": "GREEN BOOK",
    "category": "object"
  },
  {
    "name": "blue_book",
    "unicode": {"apple":"1F4D8", "google":"1F4D8", "twitter":"1F4D8"},
    "shortcode": "blue_book",
    "description": "BLUE BOOK",
    "category": "object"
  },
  {
    "name": "orange_book",
    "unicode": {"apple":"1F4D9", "google":"1F4D9", "twitter":"1F4D9"},
    "shortcode": "orange_book",
    "description": "ORANGE BOOK",
    "category": "object"
  },
  {
    "name": "notebook_with_decorative_cover",
    "unicode": {"apple":"1F4D4", "google":"1F4D4", "twitter":"1F4D4"},
    "shortcode": "notebook_with_decorative_cover",
    "description": "NOTEBOOK WITH DECORATIVE COVER",
    "category": "object"
  },
  {
    "name": "ledger",
    "unicode": {"apple":"1F4D2", "google":"1F4D2", "twitter":"1F4D2"},
    "shortcode": "ledger",
    "description": "LEDGER",
    "category": "object"
  },
  {
    "name": "books",
    "unicode": {"apple":"1F4DA", "google":"1F4DA", "twitter":"1F4DA"},
    "shortcode": "books",
    "description": "BOOKS",
    "category": "object"
  },
  {
    "name": "book",
    "unicode": {"apple":"1F4D6", "google":"1F4D6", "twitter":"1F4D6"},
    "shortcode": "book",
    "description": "OPEN BOOK",
    "category": "object"
  },
  {
    "name": "link",
    "unicode": {"apple":"1F517", "google":"1F517", "twitter":"1F517"},
    "shortcode": "link",
    "description": "LINK SYMBOL",
    "category": "object"
  },
  {
    "name": "paperclip",
    "unicode": {"apple":"1F4CE", "google":"1F4CE", "twitter":"1F4CE"},
    "shortcode": "paperclip",
    "description": "PAPERCLIP",
    "category": "object"
  },
  {
    "name": "linked_paperclips",
    "unicode": {"apple":"1F587", "google":"1F587", "twitter":"1F587"},
    "shortcode": "linked_paperclips",
    "description": "LINKED PAPERCLIPS",
    "category": "object"
  },
  {
    "name": "scissors",
    "unicode": {"apple":"2702", "google":"2702", "twitter":"2702"},
    "shortcode": "scissors",
    "description": "BLACK SCISSORS",
    "category": "object"
  },
  {
    "name": "triangular_ruler",
    "unicode": {"apple":"1F4D0", "google":"1F4D0", "twitter":"1F4D0"},
    "shortcode": "triangular_ruler",
    "description": "TRIANGULAR RULER",
    "category": "object"
  },
  {
    "name": "straight_ruler",
    "unicode": {"apple":"1F4CF", "google":"1F4CF", "twitter":"1F4CF"},
    "shortcode": "straight_ruler",
    "description": "STRAIGHT RULER",
    "category": "object"
  },
  {
    "name": "pushpin",
    "unicode": {"apple":"1F4CC", "google":"1F4CC", "twitter":"1F4CC"},
    "shortcode": "pushpin",
    "description": "PUSHPIN",
    "category": "object"
  },
  {
    "name": "round_pushpin",
    "unicode": {"apple":"1F4CD", "google":"1F4CD", "twitter":"1F4CD"},
    "shortcode": "round_pushpin",
    "description": "ROUND PUSHPIN",
    "category": "object"
  },
  {
    "name": "triangular_flag_on_post",
    "unicode": {"apple":"1F6A9", "google":"1F6A9", "twitter":"1F6A9"},
    "shortcode": "triangular_flag_on_post",
    "description": "TRIANGULAR FLAG ON POST",
    "category": "object"
  },
  {
    "name": "waving_white_flag",
    "unicode": {"apple":"1F3F3", "google":"1F3F3", "twitter":"1F3F3"},
    "shortcode": "waving_white_flag",
    "description": "WAVING WHITE FLAG",
    "category": "object"
  },
  {
    "name": "waving_black_flag",
    "unicode": {"apple":"1F3F4", "google":"1F3F4", "twitter":"1F3F4"},
    "shortcode": "waving_black_flag",
    "description": "WAVING BLACK FLAG",
    "category": "object"
  },
  {
    "name": "closed_lock_with_key",
    "unicode": {"apple":"1F510", "google":"1F510", "twitter":"1F510"},
    "shortcode": "closed_lock_with_key",
    "description": "CLOSED LOCK WITH KEY",
    "category": "object"
  },
  {
    "name": "lock",
    "unicode": {"apple":"1F512", "google":"1F512", "twitter":"1F512"},
    "shortcode": "lock",
    "description": "LOCK",
    "category": "object"
  },
  {
    "name": "unlock",
    "unicode": {"apple":"1F513", "google":"1F513", "twitter":"1F513"},
    "shortcode": "unlock",
    "description": "OPEN LOCK",
    "category": "object"
  },
  {
    "name": "lock_with_ink_pen",
    "unicode": {"apple":"1F50F", "google":"1F50F", "twitter":"1F50F"},
    "shortcode": "lock_with_ink_pen",
    "description": "LOCK WITH INK PEN",
    "category": "object"
  },
  {
    "name": "lower_left_ballpoint_pen",
    "unicode": {"apple":"1F58A", "google":"1F58A", "twitter":"1F58A"},
    "shortcode": "lower_left_ballpoint_pen",
    "description": "LOWER LEFT BALLPOINT PEN",
    "category": "object"
  },
  {
    "name": "lower_left_fountain_pen",
    "unicode": {"apple":"1F58B", "google":"1F58B", "twitter":"1F58B"},
    "shortcode": "lower_left_fountain_pen",
    "description": "LOWER LEFT FOUNTAIN PEN",
    "category": "object"
  },
  {
    "name": "black_nib",
    "unicode": {"apple":"2712", "google":"2712", "twitter":"2712"},
    "shortcode": "black_nib",
    "description": "BLACK NIB",
    "category": "folderol"
  },
  {
    "name": "memo",
    "unicode": {"apple":"1F4DD", "google":"1F4DD", "twitter":"1F4DD"},
    "shortcode": "memo",
    "description": "MEMO",
    "category": "object"
  },
  {
    "name": "pencil2",
    "unicode": {"apple":"270F-FE0F", "google":"270F-FE0F", "twitter":"270F-FE0F"},
    "shortcode": "pencil2",
    "description": "PENCIL",
    "category": "object"
  },
  {
    "name": "lower_left_crayon",
    "unicode": {"apple":"1F58D", "google":"1F58D", "twitter":"1F58D"},
    "shortcode": "lower_left_crayon",
    "description": "LOWER LEFT CRAYON",
    "category": "object"
  },
  {
    "name": "lower_left_paintbrush",
    "unicode": {"apple":"1F58C", "google":"1F58C", "twitter":"1F58C"},
    "shortcode": "lower_left_paintbrush",
    "description": "LOWER LEFT PAINTBRUSH",
    "category": "object"
  },
  {
    "name": "mag",
    "unicode": {"apple":"1F50D", "google":"1F50D", "twitter":"1F50D"},
    "shortcode": "mag",
    "description": "LEFT-POINTING MAGNIFYING GLASS",
    "category": "object"
  },
  {
    "name": "mag_right",
    "unicode": {"apple":"1F50E", "google":"1F50E", "twitter":"1F50E"},
    "shortcode": "mag_right",
    "description": "RIGHT-POINTING MAGNIFYING GLASS",
    "category": "object"
  },
  {
    "name": "heart",
    "unicode": {"apple":"2764", "google":"2764", "twitter":"2764"},
    "shortcode": "heart",
    "description": "HEAVY BLACK HEART",
    "category": "symbol"
  },
  {
    "name": "yellow_heart",
    "unicode": {"apple":"1F49B", "google":"1F49B", "twitter":"1F49B"},
    "shortcode": "yellow_heart",
    "description": "YELLOW HEART",
    "category": "symbol"
  },
  {
    "name": "green_heart",
    "unicode": {"apple":"1F49A", "google":"1F49A", "twitter":"1F49A"},
    "shortcode": "green_heart",
    "description": "GREEN HEART",
    "category": "symbol"
  },
  {
    "name": "blue_heart",
    "unicode": {"apple":"1F499", "google":"1F499", "twitter":"1F499"},
    "shortcode": "blue_heart",
    "description": "BLUE HEART",
    "category": "symbol"
  },
  {
    "name": "purple_heart",
    "unicode": {"apple":"1F49C", "google":"1F49C", "twitter":"1F49C"},
    "shortcode": "purple_heart",
    "description": "PURPLE HEART",
    "category": "symbol"
  },
  {
    "name": "broken_heart",
    "unicode": {"apple":"1F494", "google":"1F494", "twitter":"1F494"},
    "shortcode": "broken_heart",
    "description": "BROKEN HEART",
    "category": "symbol"
  },
  {
    "name": "heavy_heart_exclamation_mark_ornament",
    "unicode": {"apple":"2763", "google":"2763", "twitter":"2763"},
    "shortcode": "heavy_heart_exclamation_mark_ornament",
    "description": "HEAVY HEART EXCLAMATION MARK ORNAMENT",
    "category": "symbol"
  },
  {
    "name": "two_hearts",
    "unicode": {"apple":"1F495", "google":"1F495", "twitter":"1F495"},
    "shortcode": "two_hearts",
    "description": "TWO HEARTS",
    "category": "symbol"
  },
  {
    "name": "revolving_hearts",
    "unicode": {"apple":"1F49E", "google":"1F49E", "twitter":"1F49E"},
    "shortcode": "revolving_hearts",
    "description": "REVOLVING HEARTS",
    "category": "symbol"
  },
  {
    "name": "heartbeat",
    "unicode": {"apple":"1F493", "google":"1F493", "twitter":"1F493"},
    "shortcode": "heartbeat",
    "description": "BEATING HEART",
    "category": "symbol"
  },
  {
    "name": "heartpulse",
    "unicode": {"apple":"1F497", "google":"1F497", "twitter":"1F497"},
    "shortcode": "heartpulse",
    "description": "GROWING HEART",
    "category": "symbol"
  },
  {
    "name": "sparkling_heart",
    "unicode": {"apple":"1F496", "google":"1F496", "twitter":"1F496"},
    "shortcode": "sparkling_heart",
    "description": "SPARKLING HEART",
    "category": "symbol"
  },
  {
    "name": "cupid",
    "unicode": {"apple":"1F498", "google":"1F498", "twitter":"1F498"},
    "shortcode": "cupid",
    "description": "HEART WITH ARROW",
    "category": "symbol"
  },
  {
    "name": "gift_heart",
    "unicode": {"apple":"1F49D", "google":"1F49D", "twitter":"1F49D"},
    "shortcode": "gift_heart",
    "description": "HEART WITH RIBBON",
    "category": "symbol"
  },
  {
    "name": "heart_decoration",
    "unicode": {"apple":"1F49F", "google":"1F49F", "twitter":"1F49F"},
    "shortcode": "heart_decoration",
    "description": "HEART DECORATION",
    "category": "symbol"
  },
  {
    "name": "peace_symbol",
    "unicode": {"apple":"262E", "google":"262E", "twitter":"262E"},
    "shortcode": "peace_symbol",
    "description": "PEACE SYMBOL",
    "category": "symbol"
  },
  {
    "name": "latin_cross",
    "unicode": {"apple":"271D", "google":"271D", "twitter":"271D"},
    "shortcode": "latin_cross",
    "description": "LATIN CROSS",
    "category": "symbol"
  },
  {
    "name": "star_and_crescent",
    "unicode": {"apple":"262A", "google":"262A", "twitter":"262A"},
    "shortcode": "star_and_crescent",
    "description": "STAR AND CRESCENT",
    "category": "symbol"
  },
  {
    "name": "om_symbol",
    "unicode": {"apple":"1F549", "google":"1F549", "twitter":"1F549"},
    "shortcode": "om_symbol",
    "description": "OM SYMBOL",
    "category": "symbol"
  },
  {
    "name": "wheel_of_dharma",
    "unicode": {"apple":"2638", "google":"2638", "twitter":"2638"},
    "shortcode": "wheel_of_dharma",
    "description": "WHEEL OF DHARMA",
    "category": "symbol"
  },
  {
    "name": "star_of_david",
    "unicode": {"apple":"2721", "google":"2721", "twitter":"2721"},
    "shortcode": "star_of_david",
    "description": "STAR OF DAVID",
    "category": "symbol"
  },
  {
    "name": "six_pointed_star",
    "unicode": {"apple":"1F52F", "google":"1F52F", "twitter":"1F52F"},
    "shortcode": "six_pointed_star",
    "description": "SIX POINTED STAR WITH MIDDLE DOT",
    "category": "symbol"
  },
  {
    "name": "menorah_with_nine_branches",
    "unicode": {"apple":"1F54E", "google":"1F54E", "twitter":"1F54E"},
    "shortcode": "menorah_with_nine_branches",
    "description": "MENORAH WITH NINE BRANCHES",
    "category": "symbol"
  },
  {
    "name": "yin_yang",
    "unicode": {"apple":"262F", "google":"262F", "twitter":"262F"},
    "shortcode": "yin_yang",
    "description": "YIN YANG",
    "category": "symbol"
  },
  {
    "name": "orthodox_cross",
    "unicode": {"apple":"2626", "google":"2626", "twitter":"2626"},
    "shortcode": "orthodox_cross",
    "description": "ORTHODOX CROSS",
    "category": "symbol"
  },
  {
    "name": "place_of_worship",
    "unicode": {"apple":"1F6D0", "google":"1F6D0", "twitter":"1F6D0"},
    "shortcode": "place_of_worship",
    "description": "PLACE OF WORSHIP",
    "category": "symbol"
  },
  {
    "name": "ophiuchus",
    "unicode": {"apple":"26CE", "google":"26CE", "twitter":"26CE"},
    "shortcode": "ophiuchus",
    "description": "OPHIUCHUS",
    "category": "symbol"
  },
  {
    "name": "aries",
    "unicode": {"apple":"2648", "google":"2648", "twitter":"2648"},
    "shortcode": "aries",
    "description": "ARIES",
    "category": "symbol"
  },
  {
    "name": "taurus",
    "unicode": {"apple":"2649", "google":"2649", "twitter":"2649"},
    "shortcode": "taurus",
    "description": "TAURUS",
    "category": "symbol"
  },
  {
    "name": "gemini",
    "unicode": {"apple":"264A", "google":"264A", "twitter":"264A"},
    "shortcode": "gemini",
    "description": "GEMINI",
    "category": "symbol"
  },
  {
    "name": "cancer",
    "unicode": {"apple":"264B", "google":"264B", "twitter":"264B"},
    "shortcode": "cancer",
    "description": "CANCER",
    "category": "symbol"
  },
  {
    "name": "leo",
    "unicode": {"apple":"264C", "google":"264C", "twitter":"264C"},
    "shortcode": "leo",
    "description": "LEO",
    "category": "symbol"
  },
  {
    "name": "virgo",
    "unicode": {"apple":"264D", "google":"264D", "twitter":"264D"},
    "shortcode": "virgo",
    "description": "VIRGO",
    "category": "symbol"
  },
  {
    "name": "libra",
    "unicode": {"apple":"264E", "google":"264E", "twitter":"264E"},
    "shortcode": "libra",
    "description": "LIBRA",
    "category": "symbol"
  },
  {
    "name": "scorpius",
    "unicode": {"apple":"264F", "google":"264F", "twitter":"264F"},
    "shortcode": "scorpius",
    "description": "SCORPIUS",
    "category": "symbol"
  },
  {
    "name": "sagittarius",
    "unicode": {"apple":"2650", "google":"2650", "twitter":"2650"},
    "shortcode": "sagittarius",
    "description": "SAGITTARIUS",
    "category": "symbol"
  },
  {
    "name": "capricorn",
    "unicode": {"apple":"2651", "google":"2651", "twitter":"2651"},
    "shortcode": "capricorn",
    "description": "CAPRICORN",
    "category": "symbol"
  },
  {
    "name": "aquarius",
    "unicode": {"apple":"2652", "google":"2652", "twitter":"2652"},
    "shortcode": "aquarius",
    "description": "AQUARIUS",
    "category": "symbol"
  },
  {
    "name": "pisces",
    "unicode": {"apple":"2653", "google":"2653", "twitter":"2653"},
    "shortcode": "pisces",
    "description": "PISCES",
    "category": "symbol"
  },
  {
    "name": "id",
    "unicode": {"apple":"1F194", "google":"1F194", "twitter":"1F194"},
    "shortcode": "id",
    "description": "SQUARED ID",
    "category": "symbol"
  },
  {
    "name": "atom_symbol",
    "unicode": {"apple":"269B", "google":"269B", "twitter":"269B"},
    "shortcode": "atom_symbol",
    "description": "ATOM SYMBOL",
    "category": "symbol"
  },
  {
    "name": "u7a7a",
    "unicode": {"apple":"1F233", "google":"1F233", "twitter":"1F233"},
    "shortcode": "u7a7a",
    "description": "SQUARED CJK UNIFIED IDEOGRAPH-7A7A",
    "category": "symbol"
  },
  {
    "name": "u5272",
    "unicode": {"apple":"1F239", "google":"1F239", "twitter":"1F239"},
    "shortcode": "u5272",
    "description": "SQUARED CJK UNIFIED IDEOGRAPH-5272",
    "category": "symbol"
  },
  {
    "name": "radioactive_sign",
    "unicode": {"apple":"2622", "google":"2622", "twitter":"2622"},
    "shortcode": "radioactive_sign",
    "description": "RADIOACTIVE SIGN",
    "category": "symbol"
  },
  {
    "name": "biohazard_sign",
    "unicode": {"apple":"2623", "google":"2623", "twitter":"2623"},
    "shortcode": "biohazard_sign",
    "description": "BIOHAZARD SIGN",
    "category": "symbol"
  },
  {
    "name": "mobile_phone_off",
    "unicode": {"apple":"1F4F4", "google":"1F4F4", "twitter":"1F4F4"},
    "shortcode": "mobile_phone_off",
    "description": "MOBILE PHONE OFF",
    "category": "symbol"
  },
  {
    "name": "vibration_mode",
    "unicode": {"apple":"1F4F3", "google":"1F4F3", "twitter":"1F4F3"},
    "shortcode": "vibration_mode",
    "description": "VIBRATION MODE",
    "category": "symbol"
  },
  {
    "name": "u6709",
    "unicode": {"apple":"1F236", "google":"1F236", "twitter":"1F236"},
    "shortcode": "u6709",
    "description": "SQUARED CJK UNIFIED IDEOGRAPH-6709",
    "category": "symbol"
  },
  {
    "name": "u7121",
    "unicode": {"apple":"1F21A", "google":"1F21A", "twitter":"1F21A"},
    "shortcode": "u7121",
    "description": "SQUARED CJK UNIFIED IDEOGRAPH-7121",
    "category": "symbol"
  },
  {
    "name": "u7533",
    "unicode": {"apple":"1F238", "google":"1F238", "twitter":"1F238"},
    "shortcode": "u7533",
    "description": "SQUARED CJK UNIFIED IDEOGRAPH-7533",
    "category": "symbol"
  },
  {
    "name": "u55b6",
    "unicode": {"apple":"1F23A", "google":"1F23A", "twitter":"1F23A"},
    "shortcode": "u55b6",
    "description": "SQUARED CJK UNIFIED IDEOGRAPH-55B6",
    "category": "symbol"
  },
  {
    "name": "u6708",
    "unicode": {"apple":"1F237", "google":"1F237", "twitter":"1F237"},
    "shortcode": "u6708",
    "description": "SQUARED CJK UNIFIED IDEOGRAPH-6708",
    "category": "symbol"
  },
  {
    "name": "eight_pointed_black_star",
    "unicode": {"apple":"2734", "google":"2734", "twitter":"2734"},
    "shortcode": "eight_pointed_black_star",
    "description": "EIGHT POINTED BLACK STAR",
    "category": "symbol"
  },
  {
    "name": "vs",
    "unicode": {"apple":"1F19A", "google":"1F19A", "twitter":"1F19A"},
    "shortcode": "vs",
    "description": "SQUARED VS",
    "category": "symbol"
  },
  {
    "name": "accept",
    "unicode": {"apple":"1F251", "google":"1F251", "twitter":"1F251"},
    "shortcode": "accept",
    "description": "CIRCLED IDEOGRAPH ACCEPT",
    "category": "symbol"
  },
  {
    "name": "white_flower",
    "unicode": {"apple":"1F4AE", "google":"1F4AE", "twitter":"1F4AE"},
    "shortcode": "white_flower",
    "description": "WHITE FLOWER",
    "category": "symbol"
  },
  {
    "name": "ideograph_advantage",
    "unicode": {"apple":"1F250", "google":"1F250", "twitter":"1F250"},
    "shortcode": "ideograph_advantage",
    "description": "CIRCLED IDEOGRAPH ADVANTAGE",
    "category": "symbol"
  },
  {
    "name": "secret",
    "unicode": {"apple":"3299", "google":"3299", "twitter":"3299"},
    "shortcode": "secret",
    "description": "CIRCLED IDEOGRAPH SECRET",
    "category": "symbol"
  },
  {
    "name": "congratulations",
    "unicode": {"apple":"3297", "google":"3297", "twitter":"3297"},
    "shortcode": "congratulations",
    "description": "CIRCLED IDEOGRAPH CONGRATULATION",
    "category": "symbol"
  },
  {
    "name": "u5408",
    "unicode": {"apple":"1F234", "google":"1F234", "twitter":"1F234"},
    "shortcode": "u5408",
    "description": "SQUARED CJK UNIFIED IDEOGRAPH-5408",
    "category": "symbol"
  },
  {
    "name": "u6e80",
    "unicode": {"apple":"1F235", "google":"1F235", "twitter":"1F235"},
    "shortcode": "u6e80",
    "description": "SQUARED CJK UNIFIED IDEOGRAPH-6E80",
    "category": "symbol"
  },
  {
    "name": "u7981",
    "unicode": {"apple":"1F232", "google":"1F232", "twitter":"1F232"},
    "shortcode": "u7981",
    "description": "SQUARED CJK UNIFIED IDEOGRAPH-7981",
    "category": "symbol"
  },
  {
    "name": "a",
    "unicode": {"apple":"1F170", "google":"1F170", "twitter":"1F170"},
    "shortcode": "a",
    "description": "NEGATIVE SQUARED LATIN CAPITAL LETTER A",
    "category": "symbol"
  },
  {
    "name": "b",
    "unicode": {"apple":"1F171", "google":"1F171", "twitter":"1F171"},
    "shortcode": "b",
    "description": "NEGATIVE SQUARED LATIN CAPITAL LETTER B",
    "category": "symbol"
  },
  {
    "name": "ab",
    "unicode": {"apple":"1F18E", "google":"1F18E", "twitter":"1F18E"},
    "shortcode": "ab",
    "description": "NEGATIVE SQUARED AB",
    "category": "symbol"
  },
  {
    "name": "cl",
    "unicode": {"apple":"1F191", "google":"1F191", "twitter":"1F191"},
    "shortcode": "cl",
    "description": "SQUARED CL",
    "category": "symbol"
  },
  {
    "name": "o2",
    "unicode": {"apple":"1F17E", "google":"1F17E", "twitter":"1F17E"},
    "shortcode": "o2",
    "description": "NEGATIVE SQUARED LATIN CAPITAL LETTER O",
    "category": "symbol"
  },
  {
    "name": "sos",
    "unicode": {"apple":"1F198", "google":"1F198", "twitter":"1F198"},
    "shortcode": "sos",
    "description": "SQUARED SOS",
    "category": "symbol"
  },
  {
    "name": "no_entry",
    "unicode": {"apple":"26D4", "google":"26D4", "twitter":"26D4"},
    "shortcode": "no_entry",
    "description": "NO ENTRY",
    "category": "symbol"
  },
    {
    "name": "name_badge",
    "unicode": {"apple":"1F4DB", "google":"1F4DB", "twitter":"1F4DB"},
    "shortcode": "name_badge",
    "description": "NAME BADGE",
    "category": "symbol"
  },
  {
    "name": "no_entry_sign",
    "unicode": {"apple":"1F6AB", "google":"1F6AB", "twitter":"1F6AB"},
    "shortcode": "no_entry_sign",
    "description": "NO ENTRY SIGN",
    "category": "symbol"
  },
  {
    "name": "x",
    "unicode": {"apple":"274C", "google":"274C", "twitter":"274C"},
    "shortcode": "x",
    "description": "CROSS MARK",
    "category": "symbol"
  },
  {
    "name": "o",
    "unicode": {"apple":"2B55", "google":"2B55", "twitter":"2B55"},
    "shortcode": "o",
    "description": "HEAVY LARGE CIRCLE",
    "category": "symbol"
  },
  {
    "name": "anger",
    "unicode": {"apple":"1F4A2", "google":"1F4A2", "twitter":"1F4A2"},
    "shortcode": "anger",
    "description": "ANGER SYMBOL",
    "category": "symbol"
  },
  {
    "name": "hotsprings",
    "unicode": {"apple":"2668", "google":"2668", "twitter":"2668"},
    "shortcode": "hotsprings",
    "description": "HOT SPRINGS",
    "category": "symbol"
  },
  {
    "name": "no_pedestrians",
    "unicode": {"apple":"1F6B7", "google":"1F6B7", "twitter":"1F6B7"},
    "shortcode": "no_pedestrians",
    "description": "NO PEDESTRIANS",
    "category": "symbol"
  },
  {
    "name": "do_not_litter",
    "unicode": {"apple":"1F6AF", "google":"1F6AF", "twitter":"1F6AF"},
    "shortcode": "do_not_litter",
    "description": "DO NOT LITTER SYMBOL",
    "category": "symbol"
  },
  {
    "name": "no_bicycles",
    "unicode": {"apple":"1F6B3", "google":"1F6B3", "twitter":"1F6B3"},
    "shortcode": "no_bicycles",
    "description": "NO BICYCLES",
    "category": "symbol"
  },
  {
    "name": "non-potable_water",
    "unicode": {"apple":"1F6B1", "google":"1F6B1", "twitter":"1F6B1"},
    "shortcode": "non-potable_water",
    "description": "NON-POTABLE WATER SYMBOL",
    "category": "symbol"
  },
  {
    "name": "underage",
    "unicode": {"apple":"1F51E", "google":"1F51E", "twitter":"1F51E"},
    "shortcode": "underage",
    "description": "NO ONE UNDER EIGHTEEN SYMBOL",
    "category": "symbol"
  },
  {
    "name": "no_mobile_phones",
    "unicode": {"apple":"1F4F5", "google":"1F4F5", "twitter":"1F4F5"},
    "shortcode": "no_mobile_phones",
    "description": "NO MOBILE PHONES",
    "category": "symbol"
  },
  {
    "name": "no_smoking",
    "unicode": {"apple":"1F6AD", "google":"1F6AD", "twitter":"1F6AD"},
    "shortcode": "no_smoking",
    "description": "NO SMOKING SYMBOL",
    "category": "symbol"
  },
  {
    "name": "exclamation",
    "unicode": {"apple":"2757", "google":"2757", "twitter":"2757"},
    "shortcode": "exclamation",
    "description": "HEAVY EXCLAMATION MARK SYMBOL",
    "category": "symbol"
  },
  {
    "name": "grey_exclamation",
    "unicode": {"apple":"2755", "google":"2755", "twitter":"2755"},
    "shortcode": "grey_exclamation",
    "description": "WHITE EXCLAMATION MARK ORNAMENT",
    "category": "symbol"
  },
  {
    "name": "question",
    "unicode": {"apple":"2753", "google":"2753", "twitter":"2753"},
    "shortcode": "question",
    "description": "BLACK QUESTION MARK ORNAMENT",
    "category": "symbol"
  },
  {
    "name": "grey_question",
    "unicode": {"apple":"2754", "google":"2754", "twitter":"2754"},
    "shortcode": "grey_question",
    "description": "WHITE QUESTION MARK ORNAMENT",
    "category": "symbol"
  },
  {
    "name": "bangbang",
    "unicode": {"apple":"203C", "google":"203C", "twitter":"203C"},
    "shortcode": "bangbang",
    "description": "DOUBLE EXCLAMATION MARK",
    "category": "symbol"
  },
  {
    "name": "interrobang",
    "unicode": {"apple":"2049", "google":"2049", "twitter":"2049"},
    "shortcode": "interrobang",
    "description": "EXCLAMATION QUESTION MARK",
    "category": "symbol"
  },
  {
    "name": "100",
    "unicode": {"apple":"1F4AF", "google":"1F4AF", "twitter":"1F4AF"},
    "shortcode": "100",
    "description": "HUNDRED POINTS SYMBOL",
    "category": "symbol"
  },
  {
    "name": "low_brightness",
    "unicode": {"apple":"1F505", "google":"1F505", "twitter":"1F505"},
    "shortcode": "low_brightness",
    "description": "LOW BRIGHTNESS SYMBOL",
    "category": "symbol"
  },
  {
    "name": "high_brightness",
    "unicode": {"apple":"1F506", "google":"1F506", "twitter":"1F506"},
    "shortcode": "high_brightness",
    "description": "HIGH BRIGHTNESS SYMBOL",
    "category": "symbol"
  },
  {
    "name": "trident",
    "unicode": {"apple":"1F531", "google":"1F531", "twitter":"1F531"},
    "shortcode": "trident",
    "description": "TRIDENT EMBLEM",
    "category": "symbol"
  },
  {
    "name": "fleur_de_lis",
    "unicode": {"apple":"269C", "google":"269C", "twitter":"269C"},
    "shortcode": "fleur_de_lis",
    "description": "FLEUR-DE-LIS",
    "category": "symbol"
  },
  {
    "name": "part_alternation_mark",
    "unicode": {"apple":"303D", "google":"303D", "twitter":"303D"},
    "shortcode": "part_alternation_mark",
    "description": "PART ALTERNATION MARK",
    "category": "symbol"
  },
  {
    "name": "warning",
    "unicode": {"apple":"26A0", "google":"26A0", "twitter":"26A0"},
    "shortcode": "warning",
    "description": "WARNING SIGN",
    "category": "symbol"
  },
  {
    "name": "children_crossing",
    "unicode": {"apple":"1F6B8", "google":"1F6B8", "twitter":"1F6B8"},
    "shortcode": "children_crossing",
    "description": "CHILDREN CROSSING",
    "category": "symbol"
  },
  {
    "name": "beginner",
    "unicode": {"apple":"1F530", "google":"1F530", "twitter":"1F530"},
    "shortcode": "beginner",
    "description": "JAPANESE SYMBOL FOR BEGINNER",
    "category": "symbol"
  },
  {
    "name": "recycle",
    "unicode": {"apple":"267B", "google":"267B", "twitter":"267B"},
    "shortcode": "recycle",
    "description": "BLACK UNIVERSAL RECYCLING SYMBOL",
    "category": "symbol"
  },
  {
    "name": "u6307",
    "unicode": {"apple":"1F22F", "google":"1F22F", "twitter":"1F22F"},
    "shortcode": "u6307",
    "description": "SQUARED CJK UNIFIED IDEOGRAPH-6307",
    "category": "symbol"
  },
  {
    "name": "chart",
    "unicode": {"apple":"1F4B9", "google":"1F4B9", "twitter":"1F4B9"},
    "shortcode": "chart",
    "description": "CHART WITH UPWARDS TREND AND YEN SIGN",
    "category": "symbol"
  },
  {
    "name": "sparkle",
    "unicode": {"apple":"2747", "google":"2747", "twitter":"2747"},
    "shortcode": "sparkle",
    "description": "SPARKLE",
    "category": "symbol"
  },
  {
    "name": "eight_spoked_asterisk",
    "unicode": {"apple":"2733-FE0F", "google":"2733-FE0F", "twitter":"2733-FE0F"},
    "shortcode": "eight_spoked_asterisk",
    "description": "EIGHT SPOKED ASTERISK",
    "category": "symbol"
  },
  {
    "name": "negative_squared_cross_mark",
    "unicode": {"apple":"274E", "google":"274E", "twitter":"274E"},
    "shortcode": "negative_squared_cross_mark",
    "description": "NEGATIVE SQUARED CROSS MARK",
    "category": "symbol"
  },
  {
    "name": "white_check_mark",
    "unicode": {"apple":"2705", "google":"2705", "twitter":"2705"},
    "shortcode": "white_check_mark",
    "description": "WHITE HEAVY CHECK MARK",
    "category": "symbol"
  },
  {
    "name": "diamond_shape_with_a_dot_inside",
    "unicode": {"apple":"1F4A0", "google":"1F4A0", "twitter":"1F4A0"},
    "shortcode": "diamond_shape_with_a_dot_inside",
    "description": "DIAMOND SHAPE WITH A DOT INSIDE",
    "category": "symbol"
  },
  {
    "name": "cyclone",
    "unicode": {"apple":"1F300", "google":"1F300", "twitter":"1F300"},
    "shortcode": "cyclone",
    "description": "CYCLONE",
    "category": "symbol"
  },
  {
    "name": "loop",
    "unicode": {"apple":"27BF", "google":"27BF", "twitter":"27BF"},
    "shortcode": "loop",
    "description": "DOUBLE CURLY LOOP",
    "category": "symbol"
  },
  {
    "name": "globe_with_meridians",
    "unicode": {"apple":"1F310", "google":"1F310", "twitter":"1F310"},
    "shortcode": "globe_with_meridians",
    "description": "GLOBE WITH MERIDIANS",
    "category": "symbol"
  },
  {
    "name": "m",
    "unicode": {"apple":"24C2", "google":"24C2", "twitter":"24C2"},
    "shortcode": "m",
    "description": "CIRCLED LATIN CAPITAL LETTER M",
    "category": "symbol"
  },
  {
    "name": "atm",
    "unicode": {"apple":"1F3E7", "google":"1F3E7", "twitter":"1F3E7"},
    "shortcode": "atm",
    "description": "AUTOMATED TELLER MACHINE",
    "category": "symbol"
  },
  {
    "name": "sa",
    "unicode": {"apple":"1F202", "google":"1F202", "twitter":"1F202"},
    "shortcode": "sa",
    "description": "SQUARED KATAKANA SA",
    "category": "symbol"
  },
  {
    "name": "passport_control",
    "unicode": {"apple":"1F6C2", "google":"1F6C2", "twitter":"1F6C2"},
    "shortcode": "passport_control",
    "description": "PASSPORT CONTROL",
    "category": "symbol"
  },
  {
    "name": "customs",
    "unicode": {"apple":"1F6C3", "google":"1F6C3", "twitter":"1F6C3"},
    "shortcode": "customs",
    "description": "CUSTOMS",
    "category": "symbol"
  },
  {
    "name": "baggage_claim",
    "unicode": {"apple":"1F6C4", "google":"1F6C4", "twitter":"1F6C4"},
    "shortcode": "baggage_claim",
    "description": "BAGGAGE CLAIM",
    "category": "symbol"
  },
  {
    "name": "left_luggage",
    "unicode": {"apple":"1F6C5", "google":"1F6C5", "twitter":"1F6C5"},
    "shortcode": "left_luggage",
    "description": "LEFT LUGGAGE",
    "category": "symbol"
  },
    {
    "name": "wheelchair",
    "unicode": {"apple":"267F", "google":"267F", "twitter":"267F"},
    "shortcode": "wheelchair",
    "description": "WHEELCHAIR SYMBOL",
    "category": "symbol"
  },
  {
    "name": "wc",
    "unicode": {"apple":"1F6BE", "google":"1F6BE", "twitter":"1F6BE"},
    "shortcode": "wc",
    "description": "WATER CLOSET",
    "category": "symbol"
  },
  {
    "name": "parking",
    "unicode": {"apple":"1F17F", "google":"1F17F", "twitter":"1F17F"},
    "shortcode": "parking",
    "description": "NEGATIVE SQUARED LATIN CAPITAL LETTER P",
    "category": "symbol"
  },
  {
    "name": "potable_water",
    "unicode": {"apple":"1F6B0", "google":"1F6B0", "twitter":"1F6B0"},
    "shortcode": "potable_water",
    "description": "POTABLE WATER SYMBOL",
    "category": "symbol"
  },
  {
    "name": "mens",
    "unicode": {"apple":"1F6B9", "google":"1F6B9", "twitter":"1F6B9"},
    "shortcode": "mens",
    "description": "MENS SYMBOL",
    "category": "symbol"
  },
  {
    "name": "womens",
    "unicode": {"apple":"1F6BA", "google":"1F6BA", "twitter":"1F6BA"},
    "shortcode": "womens",
    "description": "WOMENS SYMBOL",
    "category": "symbol"
  },
  {
    "name": "baby_symbol",
    "unicode": {"apple":"1F6BC", "google":"1F6BC", "twitter":"1F6BC"},
    "shortcode": "baby_symbol",
    "description": "BABY SYMBOL",
    "category": "symbol"
  },
  {
    "name": "restroom",
    "unicode": { "apple":"1F6BB", "google":"1F6BB", "twitter":"1F6BB"},
    "shortcode": "restroom",
    "description": "RESTROOM",
    "category": "symbol"
  },
  {
    "name": "put_litter_in_its_place",
    "unicode": {"apple":"1F6AE", "google":"1F6AE", "twitter":"1F6AE"},
    "shortcode": "put_litter_in_its_place",
    "description": "PUT LITTER IN ITS PLACE SYMBOL",
    "category": "symbol"
  },
  {
    "name": "cinema",
    "unicode": {"apple":"1F3A6", "google":"1F3A6", "twitter":"1F3A6"},
    "shortcode": "cinema",
    "description": "CINEMA",
    "category": "symbol"
  },
  {
    "name": "signal_strength",
    "unicode": {"apple":"1F4F6", "google":"1F4F6", "twitter":"1F4F6"},
    "shortcode": "signal_strength",
    "description": "ANTENNA WITH BARS",
    "category": "symbol"
  },
  {
    "name": "koko",
    "unicode": { "apple":"1F201", "google":"1F201", "twitter":"1F201"},
    "shortcode": "koko",
    "description": "SQUARED KATAKANA KOKO",
    "category": "symbol"
  },
  {
    "name": "ng",
    "unicode": {"apple":"1F196", "google":"1F196", "twitter":"1F196"},
    "shortcode": "ng",
    "description": "SQUARED NG",
    "category": "symbol"
  },
  {
    "name": "ok",
    "unicode": {"apple":"1F197", "google":"1F197", "twitter":"1F197"},
    "shortcode": "ok",
    "description": "SQUARED OK",
    "category": "symbol"
  },
  {
    "name": "up",
    "unicode": {"apple":"1F199", "google":"1F199", "twitter":"1F199"},
    "shortcode": "up",
    "description": "SQUARED UP WITH EXCLAMATION MARK",
    "category": "symbol"
  },
  {
    "name": "cool",
    "unicode": {"apple":"1F192", "google":"1F192", "twitter":"1F192"},
    "shortcode": "cool",
    "description": "SQUARED COOL",
    "category": "symbol"
  },
  {
    "name": "new",
    "unicode": {"apple":"1F195", "google":"1F195", "twitter":"1F195"},
    "shortcode": "new",
    "description": "SQUARED NEW",
    "category": "symbol"
  },
  {
    "name": "free",
    "unicode": {"apple":"1F193", "google":"1F193", "twitter":"1F193"},
    "shortcode": "free",
    "description": "SQUARED FREE",
    "category": "symbol"
  },
  {
    "name": "zero",
    "unicode": {"apple":"0030-20E3", "google":"0030-20E3", "twitter":"0030-20E3"},
    "shortcode": "zero",
    "description": "KEYCAP 0",
    "category": "symbol"
  },
  {
    "name": "one",
    "unicode": {"apple":"0031-20E3", "google":"0031-20E3", "twitter":"0031-20E3"},
    "shortcode": "one",
    "description": "KEYCAP 1",
    "category": "symbol"
  },
  {
    "name": "two",
    "unicode": {"apple":"0032-20E3", "google":"0032-20E3", "twitter":"0032-20E3"},
    "shortcode": "two",
    "description": "KEYCAP 2",
    "category": "symbol"
  },
  {
    "name": "three",
    "unicode": {"apple":"0033-20E3", "google":"0033-20E3", "twitter":"0033-20E3"},
    "shortcode": "three",
    "description": "KEYCAP 3",
    "category": "symbol"
  },
  {
    "name": "four",
    "unicode": {"apple":"0034-20E3", "google":"0034-20E3", "twitter":"0034-20E3"},
    "shortcode": "four",
    "description": "KEYCAP 4",
    "category": "symbol"
  },
  {
    "name": "five",
    "unicode": {"apple":"0035-20E3", "google":"0035-20E3", "twitter":"0035-20E3"},
    "shortcode": "five",
    "description": "KEYCAP 5",
    "category": "symbol"
  },
  {
    "name": "six",
    "unicode": {"apple":"0036-20E3", "google":"0036-20E3", "twitter":"0036-20E3"},
    "shortcode": "six",
    "description": "KEYCAP 6",
    "category": "symbol"
  },
  {
    "name": "seven",
    "unicode": {"apple":"0037-20E3", "google":"0037-20E3", "twitter":"0037-20E3"},
    "shortcode": "seven",
    "description": "KEYCAP 7",
    "category": "symbol"
  },
  {
    "name": "eight",
    "unicode": {"apple":"0038-20E3", "google":"0038-20E3", "twitter":"0038-20E3"},
    "shortcode": "eight",
    "description": "KEYCAP 8",
    "category": "symbol"
  },
  {
    "name": "nine",
    "unicode": {"apple":"0039-20E3", "google":"0039-20E3", "twitter":"0039-20E3"},
    "shortcode": "nine",
    "description": "KEYCAP 9",
    "category": "symbol"
  },
  {
    "name": "keycap_ten",
    "unicode": {"apple":"1F51F", "google":"1F51F", "twitter":"1F51F"},
    "shortcode": "keycap_ten",
    "description": "KEYCAP TEN",
    "category": "symbol"
  },
  {
    "name": "1234",
    "unicode": {"apple":"1F522", "google":"1F522", "twitter":"1F522"},
    "shortcode": "1234",
    "description": "INPUT SYMBOL FOR NUMBERS",
    "category": "symbol"
  },
  {
    "name": "arrow_forward",
    "unicode": {"apple":"25B6", "google":"25B6", "twitter":"25B6"},
    "shortcode": "arrow_forward",
    "description": "BLACK RIGHT-POINTING TRIANGLE",
    "category": "symbol"
  },
  {
    "name": "double_vertical_bar",
    "unicode": {"apple":"23F8", "google":"23F8", "twitter":"23F8"},
    "shortcode": "double_vertical_bar",
    "description": "DOUBLE VERTICAL BAR"
  },
  {
    "name": "black_right_pointing_triangle_with_double_vertical_bar",
    "unicode": {"apple":"23EF", "google":"23EF", "twitter":"23EF"},
    "shortcode": "black_right_pointing_triangle_with_double_vertical_bar",
    "description": "BLACK RIGHT-POINTING TRIANGLE WITH DOUBLE VERTICAL BAR"
  },
  {
    "name": "black_square_for_stop",
    "unicode": {"apple":"23F9", "google":"23F9", "twitter":"23F9"},
    "shortcode": "black_square_for_stop",
    "description": "BLACK SQUARE FOR STOP"
  },
  {
    "name": "black_circle_for_record",
    "unicode": {"apple":"23FA", "google":"23FA", "twitter":"23FA"},
    "shortcode": "black_circle_for_record",
    "description": "BLACK CIRCLE FOR RECORD"
  },
  {
    "name": "black_right_pointing_double_triangle_with_vertical_bar",
    "unicode": {"apple":"23ED", "google":"23ED", "twitter":"23ED"},
    "shortcode": "black_right_pointing_double_triangle_with_vertical_bar",
    "description": "BLACK RIGHT-POINTING DOUBLE TRIANGLE WITH VERTICAL BAR"
  },
  {
    "name": "black_left_pointing_double_triangle_with_vertical_bar",
    "unicode": {"apple":"23EE", "google":"23EE", "twitter":"23EE"},
    "shortcode": "black_left_pointing_double_triangle_with_vertical_bar",
    "description": "BLACK LEFT-POINTING DOUBLE TRIANGLE WITH VERTICAL BAR"
  },
  {
    "name": "fast_forward",
    "unicode": {"apple":"23E9", "google":"23E9", "twitter":"23E9"},
    "shortcode": "fast_forward",
    "description": "BLACK RIGHT-POINTING DOUBLE TRIANGLE"
  },
  {
    "name": "rewind",
    "unicode": {"apple":"23EA", "google":"23EA", "twitter":"23EA"},
    "shortcode": "rewind",
    "description": "BLACK LEFT-POINTING DOUBLE TRIANGLE",
    "category": "symbol"
  },
  {
    "name": "twisted_rightwards_arrows",
    "unicode": {"apple":"1F500", "google":"1F500", "twitter":"1F500"},
    "shortcode": "twisted_rightwards_arrows",
    "description": "TWISTED RIGHTWARDS ARROWS",
    "category": "symbol"
  },
  {
    "name": "repeat",
    "unicode": {"apple":"1F501", "google":"1F501", "twitter":"1F501"},
    "shortcode": "repeat",
    "description": "CLOCKWISE RIGHTWARDS AND LEFTWARDS OPEN CIRCLE ARROWS",
    "category": "symbol"
  },
  {
    "name": "repeat_one",
    "unicode": {"apple":"1F502", "google":"1F502", "twitter":"1F502"},
    "shortcode": "repeat_one",
    "description": "CLOCKWISE RIGHTWARDS AND LEFTWARDS OPEN CIRCLE ARROWS WITH CIRCLED ONE OVERLAY",
    "category": "symbol"
  },
  {
    "name": "arrow_backward",
    "unicode": {"apple":"25C0", "google":"25C0", "twitter":"25C0"},
    "shortcode": "arrow_backward",
    "description": "BLACK LEFT-POINTING TRIANGLE",
    "category": "symbol"
  },
  {
    "name": "arrow_up_small",
    "unicode": {"apple":"1F53C", "google":"1F53C", "twitter":"1F53C"},
    "shortcode": "arrow_up_small",
    "description": "UP-POINTING SMALL RED TRIANGLE",
    "category": "symbol"
  },
  {
    "name": "arrow_down_small",
    "unicode": {"apple":"1F53D", "google":"1F53D", "twitter":"1F53D"},
    "shortcode": "arrow_down_small",
    "description": "DOWN-POINTING SMALL RED TRIANGLE",
    "category": "symbol"
  },
  {
    "name": "arrow_double_up",
    "unicode": {"apple":"23EB", "google":"23EB", "twitter":"23EB"},
    "shortcode": "arrow_double_up",
    "description": "BLACK UP-POINTING DOUBLE TRIANGLE",
    "category": "symbol"
  },
  {
    "name": "arrow_double_down",
    "unicode": {"apple":"23EC", "google":"23EC", "twitter":"23EC"},
    "shortcode": "arrow_double_down",
    "description": "BLACK DOWN-POINTING DOUBLE TRIANGLE",
    "category": "symbol"
  },
  {
    "name": "arrow_right",
    "unicode": {"apple":"27A1", "google":"27A1", "twitter":"27A1"},
    "shortcode": "arrow_right",
    "description": "BLACK RIGHTWARDS ARROW",
    "category": "symbol"
  },
  {
    "name": "arrow_left",
    "unicode": {"apple":"2B05", "google":"2B05", "twitter":"2B05"},
    "shortcode": "arrow_left",
    "description": "LEFTWARDS BLACK ARROW",
    "category": "symbol"
  },
  {
    "name": "arrow_up",
    "unicode": {"apple":"2B06", "google":"2B06", "twitter":"2B06"},
    "shortcode": "arrow_up",
    "description": "UPWARDS BLACK ARROW",
    "category": "symbol"
  },
  {
    "name": "arrow_down",
    "unicode": {"apple":"2B07", "google":"2B07", "twitter":"2B07"},
    "shortcode": "arrow_down",
    "description": "DOWNWARDS BLACK ARROW",
    "category": "symbol"
  },
  {
    "name": "arrow_upper_right",
    "unicode": {"apple":"2197", "google":"2197", "twitter":"2197"},
    "shortcode": "arrow_upper_right",
    "description": "NORTH EAST ARROW",
    "category": "symbol"
  },
  {
    "name": "arrow_lower_right",
    "unicode": {"apple":"2198", "google":"2198", "twitter":"2198"},
    "shortcode": "arrow_lower_right",
    "description": "SOUTH EAST ARROW",
    "category": "symbol"
  },
  {
    "name": "arrow_lower_left",
    "unicode": {"apple":"2199", "google":"2199", "twitter":"2199"},
    "shortcode": "arrow_lower_left",
    "description": "SOUTH WEST ARROW",
    "category": "symbol"
  },
  {
    "name": "arrow_upper_left",
    "unicode": {"apple":"2196", "google":"2196", "twitter":"2196"},
    "shortcode": "arrow_upper_left",
    "description": "NORTH WEST ARROW",
    "category": "symbol"
  },
  {
    "name": "arrow_up_down",
    "unicode": {"apple":"2195", "google":"2195", "twitter":"2195"},
    "shortcode": "arrow_up_down",
    "description": "UP DOWN ARROW",
    "category": "symbol"
  },
  {
    "name": "left_right_arrow",
    "unicode": {"apple":"2194", "google":"2194", "twitter":"2194"},
    "shortcode": "left_right_arrow",
    "description": "LEFT RIGHT ARROW",
    "category": "symbol"
  },
  {
    "name": "arrows_counterclockwise",
    "unicode": {"apple":"1F504", "google":"1F504", "twitter":"1F504"},
    "shortcode": "arrows_counterclockwise",
    "description": "ANTICLOCKWISE DOWNWARDS AND UPWARDS OPEN CIRCLE ARROWS",
    "category": "symbol"
  },
  {
    "name": "arrow_right_hook",
    "unicode": {"apple":"21AA", "google":"21AA", "twitter":"21AA"},
    "shortcode": "arrow_right_hook",
    "description": "RIGHTWARDS ARROW WITH HOOK",
    "category": "symbol"
  },
  {
    "name": "leftwards_arrow_with_hook",
    "unicode": {"apple":"21A9", "google":"21A9", "twitter":"21A9"},
    "shortcode": "leftwards_arrow_with_hook",
    "description": "LEFTWARDS ARROW WITH HOOK",
    "category": "symbol"
  },
  {
    "name": "arrow_heading_up",
    "unicode": {"apple":"2934", "google":"2934", "twitter":"2934"},
    "shortcode": "arrow_heading_up",
    "description": "ARROW POINTING RIGHTWARDS THEN CURVING UPWARDS",
    "category": "symbol"
  },
  {
    "name": "arrow_heading_down",
    "unicode": {"apple":"2935", "google":"2935", "twitter":"2935"},
    "shortcode": "arrow_heading_down",
    "description": "ARROW POINTING RIGHTWARDS THEN CURVING DOWNWARDS",
    "category": "symbol"
  },
  {
    "name": "arrows_clockwise",
    "unicode": {"apple":"1F503", "google":"1F503", "twitter":"1F503"},
    "shortcode": "arrows_clockwise",
    "description": "CLOCKWISE DOWNWARDS AND UPWARDS OPEN CIRCLE ARROWS",
    "category": "symbol"
  },
  {
    "name": "hash",
    "unicode": {"apple":"0023-20E3", "google":"0023-20E3", "twitter":"0023-20E3"},
    "shortcode": "hash",
    "description": "HASH KEY",
    "category": "symbol"
  },
  {
    "name": "information_source",
    "unicode": {"apple":"2139", "google":"2139", "twitter":"2139"},
    "shortcode": "information_source",
    "description": "INFORMATION SOURCE",
    "category": "symbol"
  },
  {
    "name": "abc",
    "unicode": {"apple":"1F524", "google":"1F524", "twitter":"1F524"},
    "shortcode": "abc",
    "description": "INPUT SYMBOL FOR LATIN LETTERS",
    "category": "symbol"
  },
  {
    "name": "abcd",
    "unicode": {"apple":"1F521", "google":"1F521", "twitter":"1F521"},
    "shortcode": "abcd",
    "description": "INPUT SYMBOL FOR LATIN SMALL LETTERS",
    "category": "symbol"
  },
  {
    "name": "capital_abcd",
    "unicode": {"apple":"1F520", "google":"1F520", "twitter":"1F520"},
    "shortcode": "capital_abcd",
    "description": "INPUT SYMBOL FOR LATIN CAPITAL LETTERS",
    "category": "symbol"
  },
  {
    "name": "symbols",
    "unicode": {"apple":"1F523", "google":"1F523", "twitter":"1F523"},
    "shortcode": "symbols",
    "description": "INPUT SYMBOL FOR SYMBOLS",
    "category": "symbol"
  },
  {
    "name": "musical_note",
    "unicode": {"apple":"1F3B5", "google":"1F3B5", "twitter":"1F3B5"},
    "shortcode": "musical_note",
    "description": "MUSICAL NOTE",
    "category": "symbol"
  },
  {
    "name": "notes",
    "unicode": {"apple":"1F3B6", "google":"1F3B6", "twitter":"1F3B6"},
    "shortcode": "notes",
    "description": "MULTIPLE MUSICAL NOTES",
    "category": "symbol"
  },
  {
    "name": "wavy_dash",
    "unicode": {"apple":"3030", "google":"3030", "twitter":"3030"},
    "shortcode": "wavy_dash",
    "description": "WAVY DASH",
    "category": "symbol"
  },
  {
    "name": "curly_loop",
    "unicode": {"apple":"27B0", "google":"27B0", "twitter":"27B0"},
    "shortcode": "curly_loop",
    "description": "CURLY LOOP",
    "category": "symbol"
  },
  {
    "name": "heavy_check_mark",
    "unicode": {"apple":"2714", "google":"2714", "twitter":"2714"},
    "shortcode": "heavy_check_mark",
    "description": "HEAVY CHECK MARK",
    "category": "symbol"
  },
  {
    "name": "heavy_plus_sign",
    "unicode": {"apple":"2795", "google":"2795", "twitter":"2795"},
    "shortcode": "heavy_plus_sign",
    "description": "HEAVY PLUS SIGN",
    "category": "symbol"
  },
  {
    "name": "heavy_minus_sign",
    "unicode": {"apple":"2796", "google":"2796", "twitter":"2796"},
    "shortcode": "heavy_minus_sign",
    "description": "HEAVY MINUS SIGN",
    "category": "symbol"
  },
  {
    "name": "heavy_multiplication_x",
    "unicode": {"apple":"2716", "google":"2716", "twitter":"2716"},
    "shortcode": "heavy_multiplication_x",
    "description": "HEAVY MULTIPLICATION X",
    "category": "symbol"
  },
  {
    "name": "heavy_division_sign",
    "unicode": {"apple":"2797", "google":"2797", "twitter":"2797"},
    "shortcode": "heavy_division_sign",
    "description": "HEAVY DIVISION SIGN",
    "category": "symbol"
  },
  {
    "name": "heavy_dollar_sign",
    "unicode": {"apple":"1F4B2", "google":"1F4B2", "twitter":"1F4B2"},
    "shortcode": "heavy_dollar_sign",
    "description": "HEAVY DOLLAR SIGN",
    "category": "symbol"
  },
  {
    "name": "copyright",
    "unicode": {"apple":"00A9", "google":"00A9-FE0F", "twitter":"00A9-FE0F"},
    "shortcode": "copyright",
    "description": "COPYRIGHT SIGN",
    "category": "symbol"
  },
  {
    "name": "registered",
    "unicode": {"apple":"00AE", "google":"00AE-FE0F", "twitter":"00AE-FE0F"},
    "shortcode": "registered",
    "description": "REGISTERED SIGN",
    "category": "symbol"
  },
  {
    "name": "tm",
    "unicode": {"apple":"2122", "google":"2122", "twitter":"2122"},
    "shortcode": "tm",
    "description": "TRADE MARK SIGN",
    "category": "symbol"
  },
  {
    "name": "end",
    "unicode": {"apple":"1F51A", "google":"1F51A", "twitter":"1F51A"},
    "shortcode": "end",
    "description": "END WITH LEFTWARDS ARROW ABOVE",
    "category": "symbol"
  },
  {
    "name": "back",
    "unicode": {"apple":"1F519", "google":"1F519", "twitter":"1F519"},
    "shortcode": "back",
    "description": "BACK WITH LEFTWARDS ARROW ABOVE",
    "category": "symbol"
  },
  {
    "name": "on",
    "unicode": {"apple":"1F51B", "google":"1F51B", "twitter":"1F51B"},
    "shortcode": "on",
    "description": "ON WITH EXCLAMATION MARK WITH LEFT RIGHT ARROW ABOVE",
    "category": "symbol"
  },
  {
    "name": "top",
    "unicode": {"apple":"1F51D", "google":"1F51D", "twitter":"1F51D"},
    "shortcode": "top",
    "description": "TOP WITH UPWARDS ARROW ABOVE",
    "category": "symbol"
  },
  {
    "name": "soon",
    "unicode": {"apple":"1F51C", "google":"1F51C", "twitter":"1F51C"},
    "shortcode": "soon",
    "description": "SOON WITH RIGHTWARDS ARROW ABOVE",
    "category": "symbol"
  },
  {
    "name": "ballot_box_with_check",
    "unicode": {"apple":"2611", "google":"2611", "twitter":"2611"},
    "shortcode": "ballot_box_with_check",
    "description": "BALLOT BOX WITH CHECK",
    "category": "symbol"
  },
  {
    "name": "radio_button",
    "unicode": {"apple":"1F518", "google":"1F518", "twitter":"1F518"},
    "shortcode": "radio_button",
    "description": "RADIO BUTTON",
    "category": "symbol"
  },
  {
    "name": "white_circle",
    "unicode": {"apple":"26AA", "google":"26AA", "twitter":"26AA"},
    "shortcode": "white_circle",
    "description": "MEDIUM WHITE CIRCLE",
    "category": "symbol"
  },
  {
    "name": "black_circle",
    "unicode": {"apple":"26AB", "google":"26AB", "twitter":"26AB"},
    "shortcode": "black_circle",
    "description": "MEDIUM BLACK CIRCLE",
    "category": "symbol"
  },
  {
    "name": "red_circle",
    "unicode": {"apple":"1F534", "google":"1F534", "twitter":"1F534"},
    "shortcode": "red_circle",
    "description": "LARGE RED CIRCLE",
    "category": "symbol"
  },
  {
    "name": "large_blue_circle",
    "unicode": {"apple":"1F535", "google":"1F535", "twitter":"1F535"},
    "shortcode": "large_blue_circle",
    "description": "LARGE BLUE CIRCLE",
    "category": "symbol"
  },
  {
    "name": "small_orange_diamond",
    "unicode": {"apple":"1F538", "google":"1F538", "twitter":"1F538"},
    "shortcode": "small_orange_diamond",
    "description": "SMALL ORANGE DIAMOND",
    "category": "symbol"
  },
  {
    "name": "small_blue_diamond",
    "unicode": {"apple":"1F539", "google":"1F539", "twitter":"1F539"},
    "shortcode": "small_blue_diamond",
    "description": "SMALL BLUE DIAMOND",
    "category": "symbol"
  },
  {
    "name": "large_orange_diamond",
    "unicode": {"apple":"1F536", "google":"1F536", "twitter":"1F536"},
    "shortcode": "large_orange_diamond",
    "description": "LARGE ORANGE DIAMOND",
    "category": "symbol"
  },
  {
    "name": "large_blue_diamond",
    "unicode": {"apple":"1F537", "google":"1F537", "twitter":"1F537"},
    "shortcode": "large_blue_diamond",
    "description": "LARGE BLUE DIAMOND",
    "category": "symbol"
  },
  {
    "name": "small_red_triangle",
    "unicode": {"apple":"1F53A", "google":"1F53A", "twitter":"1F53A"},
    "shortcode": "small_red_triangle",
    "description": "UP-POINTING RED TRIANGLE",
    "category": "symbol"
  },
  {
    "name": "black_small_square",
    "unicode": {"apple":"25AA", "google":"25AA", "twitter":"25AA"},
    "shortcode": "black_small_square",
    "description": "BLACK SMALL SQUARE",
    "category": "symbol"
  },
  {
    "name": "white_small_square",
    "unicode": {"apple":"25AB", "google":"25AB", "twitter":"25AB"},
    "shortcode": "white_small_square",
    "description": "WHITE SMALL SQUARE",
    "category": "symbol"
  },
  {
    "name": "black_large_square",
    "unicode": {"apple":"2B1B", "google":"2B1B", "twitter":"2B1B"},
    "shortcode": "black_large_square",
    "description": "BLACK LARGE SQUARE",
    "category": "symbol"
  },
  {
    "name": "white_large_square",
    "unicode": {"apple":"2B1C", "google":"2B1C", "twitter":"2B1C"},
    "shortcode": "white_large_square",
    "description": "WHITE LARGE SQUARE",
    "category": "symbol"
  },
  {
    "name": "small_red_triangle_down",
    "unicode": {"apple":"1F53B", "google":"1F53B", "twitter":"1F53B"},
    "shortcode": "small_red_triangle_down",
    "description": "DOWN-POINTING RED TRIANGLE",
    "category": "symbol"
  },
  {
    "name": "black_medium_square",
    "unicode": {"apple":"25FC", "google":"25FC", "twitter":"25FC"},
    "shortcode": "black_medium_square",
    "description": "BLACK MEDIUM SQUARE",
    "category": "symbol"
  },
  {
    "name": "white_medium_square",
    "unicode": {"apple":"25FB", "google":"25FB", "twitter":"25FB"},
    "shortcode": "white_medium_square",
    "description": "WHITE MEDIUM SQUARE",
    "category": "symbol"
  },
  {
    "name": "black_medium_small_square",
    "unicode": {"apple":"25FE", "google":"25FE", "twitter":"25FE"},
    "shortcode": "black_medium_small_square",
    "description": "BLACK MEDIUM SMALL SQUARE",
    "category": "symbol"
  },
  {
    "name": "white_medium_small_square",
    "unicode": {"apple":"25FD", "google":"25FD", "twitter":"25FD"},
    "shortcode": "white_medium_small_square",
    "description": "WHITE MEDIUM SMALL SQUARE",
    "category": "symbol"
  },
  {
    "name": "black_square_button",
    "unicode": {"apple":"1F532", "google":"1F532", "twitter":"1F532"},
    "shortcode": "black_square_button",
    "description": "BLACK SQUARE BUTTON",
    "category": "symbol"
  },
  {
    "name": "white_square_button",
    "unicode": {"apple":"1F533", "google":"1F533", "twitter":"1F533"},
    "shortcode": "white_square_button",
    "description": "WHITE SQUARE BUTTON",
    "category": "symbol"
  },
  {
    "name": "speaker",
    "unicode": {"apple":"1F508", "google":"1F508", "twitter":"1F508"},
    "shortcode": "speaker",
    "description": "SPEAKER",
    "category": "symbol"
  },
  {
    "name": "sound",
    "unicode": {"apple":"1F509", "google":"1F509", "twitter":"1F509"},
    "shortcode": "sound",
    "description": "SPEAKER WITH ONE SOUND WAVE",
    "category": "symbol"
  },
  {
    "name": "loud_sound",
    "unicode": {"apple":"1F50A", "google":"1F50A", "twitter":"1F50A"},
    "shortcode": "loud_sound",
    "description": "SPEAKER WITH THREE SOUND WAVES",
    "category": "symbol"
  },
  {
    "name": "mute",
    "unicode": {"apple":"1F507", "google":"1F507", "twitter":"1F507"},
    "shortcode": "mute",
    "description": "SPEAKER WITH CANCELLATION STROKE",
    "category": "symbol"
  },
  {
    "name": "mega",
    "unicode": {"apple":"1F4E3", "google":"1F4E3", "twitter":"1F4E3"},
    "shortcode": "mega",
    "description": "CHEERING MEGAPHONE",
    "category": "symbol"
  },
  {
    "name": "loudspeaker",
    "unicode": {"apple":"1F4E2", "google":"1F4E2", "twitter":"1F4E2"},
    "shortcode": "loudspeaker",
    "description": "PUBLIC ADDRESS LOUDSPEAKER",
    "category": "symbol"
  },
  {
    "name": "bell",
    "unicode": {"apple":"1F514", "google":"1F514", "twitter":"1F514"},
    "shortcode": "bell",
    "description": "BELL",
    "category": "symbol"
  },
  {
    "name": "no_bell",
    "unicode": {"apple":"1F515", "google":"1F515", "twitter":"1F515"},
    "shortcode": "no_bell",
    "description": "BELL WITH CANCELLATION STROKE",
    "category": "symbol"
  },
  {
    "name": "black_joker",
    "unicode": {"apple":"1F0CF", "google":"1F0CF", "twitter":"1F0CF"},
    "shortcode": "black_joker",
    "description": "PLAYING CARD BLACK JOKER",
    "category": "symbol"
  },
  {
    "name": "mahjong",
    "unicode": {"apple":"1F004", "google":"1F004", "twitter":"1F004"},
    "shortcode": "mahjong",
    "description": "MAHJONG TILE RED DRAGON",
    "category": "symbol"
  },
  {
    "name": "spades",
    "unicode": {"apple":"2660", "google":"2660", "twitter":"2660"},
    "shortcode": "spades",
    "description": "BLACK SPADE SUIT",
    "category": "symbol"
  },
  {
    "name": "clubs",
    "unicode": {"apple":"2663", "google":"2663", "twitter":"2663"},
    "shortcode": "clubs",
    "description": "BLACK CLUB SUIT",
    "category": "symbol"
  },
  {
    "name": "hearts",
    "unicode": {"apple":"2665", "google":"2665", "twitter":"2665"},
    "shortcode": "hearts",
    "description": "BLACK HEART SUIT",
    "category": "symbol"
  },
  {
    "name": "diamonds",
    "unicode": {"apple":"2666", "google":"2666", "twitter":"2666"},
    "shortcode": "diamonds",
    "description": "BLACK DIAMOND SUIT",
    "category": "symbol"
  },
  {
    "name": "flower_playing_cards",
    "unicode": {"apple":"1F3B4", "google":"1F3B4", "twitter":"1F3B4"},
    "shortcode": "flower_playing_cards",
    "description": "FLOWER PLAYING CARDS",
    "category": "symbol"
  },
  {
    "name": "thought_balloon",
    "unicode": {"apple":"1F4AD", "google":"1F4AD", "twitter":"1F4AD"},
    "shortcode": "thought_balloon",
    "description": "THOUGHT BALLOON",
    "category": "symbol"
  },
  {
    "name": "right_anger_bubble",
    "unicode": {"apple":"1F5EF", "google":"1F5EF", "twitter":"1F5EF"},
    "shortcode": "right_anger_bubble",
    "description": "RIGHT ANGER BUBBLE"
  },
  {
    "name": "speech_balloon",
    "unicode": {"apple":"1F4AC", "google":"1F4AC", "twitter":"1F4AC"},
    "shortcode": "speech_balloon",
    "description": "SPEECH BALLOON",
    "category": "symbol"
  },
  {
    "name": "left_speech_bubble",
    "unicode": {"apple":"1F5E8", "google":"1F5E8", "twitter":"1F5E8"},
    "shortcode": "left_speech_bubble",
    "description": "LEFT SPEECH BUBBLE"
  },
  {
    "name": "clock1",
    "unicode": {"apple":"1F550", "google":"1F550", "twitter":"1F550"},
    "shortcode": "clock1",
    "description": "CLOCK FACE ONE OCLOCK",
    "category": "symbol"
  },
  {
    "name": "clock2",
    "unicode": {"apple":"1F551", "google":"1F551", "twitter":"1F551"},
    "shortcode": "clock2",
    "description": "CLOCK FACE TWO OCLOCK",
    "category": "symbol"
  },
  {
    "name": "clock3",
    "unicode": {"apple":"1F552", "google":"1F552", "twitter":"1F552"},
    "shortcode": "clock3",
    "description": "CLOCK FACE THREE OCLOCK",
    "category": "symbol"
  },
  {
    "name": "clock4",
    "unicode": {"apple":"1F553", "google":"1F553", "twitter":"1F553"},
    "shortcode": "clock4",
    "description": "CLOCK FACE FOUR OCLOCK",
    "category": "symbol"
  },
  {
    "name": "clock5",
    "unicode": {"apple":"1F554", "google":"1F554", "twitter":"1F554"},
    "shortcode": "clock5",
    "description": "CLOCK FACE FIVE OCLOCK",
    "category": "symbol"
  },
  {
    "name": "clock6",
    "unicode": {"apple":"1F555", "google":"1F555", "twitter":"1F555"},
    "shortcode": "clock6",
    "description": "CLOCK FACE SIX OCLOCK",
    "category": "symbol"
  },
  {
    "name": "clock7",
    "unicode": {"apple":"1F556", "google":"1F556", "twitter":"1F556"},
    "shortcode": "clock7",
    "description": "CLOCK FACE SEVEN OCLOCK",
    "category": "symbol"
  },
  {
    "name": "clock8",
    "unicode": {"apple":"1F557", "google":"1F557", "twitter":"1F557"},
    "shortcode": "clock8",
    "description": "CLOCK FACE EIGHT OCLOCK",
    "category": "symbol"
  },
  {
    "name": "clock9",
    "unicode": {"apple":"1F558", "google":"1F558", "twitter":"1F558"},
    "shortcode": "clock9",
    "description": "CLOCK FACE NINE OCLOCK",
    "category": "symbol"
  },
  {
    "name": "clock10",
    "unicode": {"apple":"1F559", "google":"1F559", "twitter":"1F559"},
    "shortcode": "clock10",
    "description": "CLOCK FACE TEN OCLOCK",
    "category": "symbol"
  },
  {
    "name": "clock11",
    "unicode": {"apple":"1F55A", "google":"1F55A", "twitter":"1F55A"},
    "shortcode": "clock11",
    "description": "CLOCK FACE ELEVEN OCLOCK",
    "category": "symbol"
  },
  {
    "name": "clock12",
    "unicode": {"apple":"1F55B", "google":"1F55B", "twitter":"1F55B"},
    "shortcode": "clock12",
    "description": "CLOCK FACE TWELVE OCLOCK",
    "category": "symbol"
  },
  {
    "name": "clock130",
    "unicode": {"apple":"1F55C", "google":"1F55C", "twitter":"1F55C"},
    "shortcode": "clock130",
    "description": "CLOCK FACE ONE-THIRTY",
    "category": "symbol"
  },
  {
    "name": "clock230",
    "unicode": {"apple":"1F55D", "google":"1F55D", "twitter":"1F55D"},
    "shortcode": "clock230",
    "description": "CLOCK FACE TWO-THIRTY",
    "category": "symbol"
  },
  {
    "name": "clock330",
    "unicode": {"apple":"1F55E", "google":"1F55E", "twitter":"1F55E"},
    "shortcode": "clock330",
    "description": "CLOCK FACE THREE-THIRTY",
    "category": "symbol"
  },
  {
    "name": "clock430",
    "unicode": {"apple":"1F55F", "google":"1F55F", "twitter":"1F55F"},
    "shortcode": "clock430",
    "description": "CLOCK FACE FOUR-THIRTY",
    "category": "symbol"
  },
  {
    "name": "clock530",
    "unicode": {"apple":"1F560", "google":"1F560", "twitter":"1F560"},
    "shortcode": "clock530",
    "description": "CLOCK FACE FIVE-THIRTY",
    "category": "symbol"
  },
  {
    "name": "clock630",
    "unicode": {"apple":"1F561", "google":"1F561", "twitter":"1F561"},
    "shortcode": "clock630",
    "description": "CLOCK FACE SIX-THIRTY",
    "category": "symbol"
  },
  {
    "name": "clock730",
    "unicode": {"apple":"1F562", "google":"1F562", "twitter":"1F562"},
    "shortcode": "clock730",
    "description": "CLOCK FACE SEVEN-THIRTY",
    "category": "symbol"
  },
  {
    "name": "clock830",
    "unicode": {"apple":"1F563", "google":"1F563", "twitter":"1F563"},
    "shortcode": "clock830",
    "description": "CLOCK FACE EIGHT-THIRTY",
    "category": "symbol"
  },
  {
    "name": "clock930",
    "unicode": {"apple":"1F564", "google":"1F564", "twitter":"1F564"},
    "shortcode": "clock930",
    "description": "CLOCK FACE NINE-THIRTY",
    "category": "symbol"
  },
  {
    "name": "clock1030",
    "unicode": {"apple":"1F565", "google":"1F565", "twitter":"1F565"},
    "shortcode": "clock1030",
    "description": "CLOCK FACE TEN-THIRTY",
    "category": "symbol"
  },
  {
    "name": "clock1130",
    "unicode": {"apple":"1F566", "google":"1F566", "twitter":"1F566"},
    "shortcode": "clock1130",
    "description": "CLOCK FACE ELEVEN-THIRTY",
    "category": "symbol"
  },
  {
    "name": "clock1230",
    "unicode": {"apple":"1F567", "google":"1F567", "twitter":"1F567"},
    "shortcode": "clock1230",
    "description": "CLOCK FACE TWELVE-THIRTY",
    "category": "symbol"
  },
  {
    "name": "flag-ac",
    "unicode": {"apple":"1F1E6-1F1E8", "google":"1F1E6-1F1E8", "twitter":"1F1E6-1F1E8"},
    "shortcode": "flag-ac",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS AC",
    "category": "flag"
  },
  {
    "name": "flag-ad",
    "unicode": {"apple":"1F1E6-1F1E9", "google":"1F1E6-1F1E9", "twitter":"1F1E6-1F1E9"},
    "shortcode": "flag-ad",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS AD",
    "category": "flag"
  },
  {
    "name": "flag-ae",
    "unicode": {"apple":"1F1E6-1F1EA", "google":"1F1E6-1F1EA", "twitter":"1F1E6-1F1EA"},
    "shortcode": "flag-ae",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS AE",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-af",
    "unicode": {"apple":"1F1E6-1F1EB", "google":"1F1E6-1F1EB", "twitter":"1F1E6-1F1EB"},
    "shortcode": "flag-af",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS AF",
    "category": "flag"
  },
  {
    "name": "flag-ag",
    "unicode": {"apple":"1F1E6-1F1EC", "google":"1F1E6-1F1EC", "twitter":"1F1E6-1F1EC"},
    "shortcode": "flag-ag",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS AG",
    "category": "flag"
  },
  {
    "name": "flag-ai",
    "unicode": {"apple":"1F1E6-1F1EE", "google":"1F1E6-1F1EE", "twitter":"1F1E6-1F1EE"},
    "shortcode": "flag-ai",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS AI",
    "category": "flag"
  },
  {
    "name": "flag-al",
    "unicode": {"apple":"1F1E6-1F1F1", "google":"1F1E6-1F1F1", "twitter":"1F1E6-1F1F1"},
    "shortcode": "flag-al",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS AL",
    "category": "flag"
  },
  {
    "name": "flag-am",
    "unicode": {"apple":"1F1E6-1F1F2", "google":"1F1E6-1F1F2", "twitter":"1F1E6-1F1F2"},
    "shortcode": "flag-am",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS AM",
    "category": "flag"
  },
  {
    "name": "flag-ao",
    "unicode": {"apple":"1F1E6-1F1F4", "google":"1F1E6-1F1F4", "twitter":"1F1E6-1F1F4"},
    "shortcode": "flag-ao",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS AO",
    "category": "flag"
  },
  {
    "name": "flag-aq",
    "unicode": {"apple":"1F1E6-1F1F6", "google":"1F1E6-1F1F6", "twitter":"1F1E6-1F1F6"},
    "shortcode": "flag-aq",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS AQ",
    "category": "flag"
  },
  {
    "name": "flag-ar",
    "unicode": {"apple":"1F1E6-1F1F7", "google":"1F1E6-1F1F7", "twitter":"1F1E6-1F1F7"},
    "shortcode": "flag-ar",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS AR",
    "category": "flag"
  },
  {
    "name": "flag-as",
    "unicode": {"apple":"1F1E6-1F1F8", "google":"1F1E6-1F1F8", "twitter":"1F1E6-1F1F8"},
    "shortcode": "flag-as",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS AS",
    "category": "flag"
  },
  {
    "name": "flag-at",
    "unicode": {"apple":"1F1E6-1F1F9", "google":"1F1E6-1F1F9", "twitter":"1F1E6-1F1F9"},
    "shortcode": "flag-at",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS AT",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-au",
    "unicode": {"apple":"1F1E6-1F1FA", "google":"1F1E6-1F1FA", "twitter":"1F1E6-1F1FA"},
    "shortcode": "flag-au",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS AU",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-aw",
    "unicode": {"apple":"1F1E6-1F1FC", "google":"1F1E6-1F1FC", "twitter":"1F1E6-1F1FC"},
    "shortcode": "flag-aw",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS AW",
    "category": "flag"
  },
  {
    "name": "flag-ax",
    "unicode": {"apple":"1F1E6-1F1FD", "google":"1F1E6-1F1FD", "twitter":"1F1E6-1F1FD"},
    "shortcode": "flag-ax",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS AX",
    "category": "flag"
  },
  {
    "name": "flag-az",
    "unicode": {"apple":"1F1E6-1F1FF", "google":"1F1E6-1F1FF", "twitter":"1F1E6-1F1FF"},
    "shortcode": "flag-az",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS AZ",
    "category": "flag"
  },
  {
    "name": "flag-ba",
    "unicode": {"apple":"1F1E7-1F1E6", "google":"1F1E7-1F1E6", "twitter":"1F1E7-1F1E6"},
    "shortcode": "flag-ba",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS BA",
    "category": "flag"
  },
  {
    "name": "flag-bb",
    "unicode": {"apple":"1F1E7-1F1E7", "google":"1F1E7-1F1E7", "twitter":"1F1E7-1F1E7"},
    "shortcode": "flag-bb",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS BB",
    "category": "flag"
  },
  {
    "name": "flag-bd",
    "unicode": {"apple":"1F1E7-1F1E9", "google":"1F1E7-1F1E9", "twitter":"1F1E7-1F1E9"},
    "shortcode": "flag-bd",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS BD",
    "category": "flag"
  },
  {
    "name": "flag-be",
    "unicode": {"apple":"1F1E7-1F1EA", "google":"1F1E7-1F1EA", "twitter":"1F1E7-1F1EA"},
    "shortcode": "flag-be",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS BE",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-bf",
    "unicode": {"apple":"1F1E7-1F1EB", "google":"1F1E7-1F1EB", "twitter":"1F1E7-1F1EB"},
    "shortcode": "flag-bf",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS BF",
    "category": "flag"
  },
  {
    "name": "flag-bg",
    "unicode": {"apple":"1F1E7-1F1EC", "google":"1F1E7-1F1EC", "twitter":"1F1E7-1F1EC"},
    "shortcode": "flag-bg",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS BG",
    "category": "flag"
  },
  {
    "name": "flag-bh",
    "unicode": {"apple":"1F1E7-1F1ED", "google":"1F1E7-1F1ED", "twitter":"1F1E7-1F1ED"},
    "shortcode": "flag-bh",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS BH",
    "category": "flag"
  },
  {
    "name": "flag-bi",
    "unicode": {"apple":"1F1E7-1F1EE", "google":"1F1E7-1F1EE", "twitter":"1F1E7-1F1EE"},
    "shortcode": "flag-bi",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS BI",
    "category": "flag"
  },
  {
    "name": "flag-bj",
    "unicode": {"apple":"1F1E7-1F1EF", "google":"1F1E7-1F1EF", "twitter":"1F1E7-1F1EF"},
    "shortcode": "flag-bj",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS BJ",
    "category": "flag"
  },
  {
    "name": "flag-bl",
    "unicode": {"apple":"1F1E7-1F1F1", "google":"1F1E7-1F1F1", "twitter":"1F1E7-1F1F1"},
    "shortcode": "flag-bl",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS BL",
    "category": "flag"
  },
  {
    "name": "flag-bm",
    "unicode": {"apple":"1F1E7-1F1F2", "google":"1F1E7-1F1F2", "twitter":"1F1E7-1F1F2"},
    "shortcode": "flag-bm",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS BM",
    "category": "flag"
  },
  {
    "name": "flag-bn",
    "unicode": {"apple":"1F1E7-1F1F3", "google":"1F1E7-1F1F3", "twitter":"1F1E7-1F1F3"},
    "shortcode": "flag-bn",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS BN",
    "category": "flag"
  },
  {
    "name": "flag-bo",
    "unicode": {"apple":"1F1E7-1F1F4", "google":"1F1E7-1F1F4", "twitter":"1F1E7-1F1F4"},
    "shortcode": "flag-bo",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS BO",
    "category": "flag"
  },
  {
    "name": "flag-bq",
    "unicode": {"apple":"1F1E7-1F1F6", "google":"1F1E7-1F1F6", "twitter":"1F1E7-1F1F6"},
    "shortcode": "flag-bq",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS BQ",
    "category": "flag"
  },
  {
    "name": "flag-br",
    "unicode": {"apple":"1F1E7-1F1F7", "google":"1F1E7-1F1F7", "twitter":"1F1E7-1F1F7"},
    "shortcode": "flag-br",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS BR",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-bs",
    "unicode": {"apple":"1F1E7-1F1F8", "google":"1F1E7-1F1F8", "twitter":"1F1E7-1F1F8"},
    "shortcode": "flag-bs",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS BS",
    "category": "flag"
  },
  {
    "name": "flag-bt",
    "unicode": {"apple":"1F1E7-1F1F9", "google":"1F1E7-1F1F9", "twitter":"1F1E7-1F1F9"},
    "shortcode": "flag-bt",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS BT",
    "category": "flag"
  },
  {
    "name": "flag-bv",
    "unicode": {"apple":"1F1E7-1F1FB", "google":"1F1E7-1F1FB", "twitter":"1F1E7-1F1FB"},
    "shortcode": "flag-bv",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS BV",
    "category": "flag"
  },
  {
    "name": "flag-bw",
    "unicode": {"apple":"1F1E7-1F1FC", "google":"1F1E7-1F1FC", "twitter":"1F1E7-1F1FC"},
    "shortcode": "flag-bw",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS BW",
    "category": "flag"
  },
  {
    "name": "flag-by",
    "unicode": {"apple":"1F1E7-1F1FE", "google":"1F1E7-1F1FE", "twitter":"1F1E7-1F1FE"},
    "shortcode": "flag-by",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS BY",
    "category": "flag"
  },
  {
    "name": "flag-bz",
    "unicode": {"apple":"1F1E7-1F1FF", "google":"1F1E7-1F1FF", "twitter":"1F1E7-1F1FF"},
    "shortcode": "flag-bz",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS BZ",
    "category": "flag"
  },
  {
    "name": "flag-ca",
    "unicode": {"apple":"1F1E8-1F1E6", "google":"1F1E8-1F1E6", "twitter":"1F1E8-1F1E6"},
    "shortcode": "flag-ca",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS CA",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-cc",
    "unicode": {"apple":"1F1E8-1F1E8", "google":"1F1E8-1F1E8", "twitter":"1F1E8-1F1E8"},
    "shortcode": "flag-cc",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS CC",
    "category": "flag"
  },
  {
    "name": "flag-cd",
    "unicode": {"apple":"1F1E8-1F1E9", "google":"1F1E8-1F1E9", "twitter":"1F1E8-1F1E9"},
    "shortcode": "flag-cd",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS CD",
    "category": "flag"
  },
  {
    "name": "flag-cf",
    "unicode": {"apple":"1F1E8-1F1EB", "google":"1F1E8-1F1EB", "twitter":"1F1E8-1F1EB"},
    "shortcode": "flag-cf",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS CF",
    "category": "flag"
  },
  {
    "name": "flag-cg",
    "unicode": {"apple":"1F1E8-1F1EC", "google":"1F1E8-1F1EC", "twitter":"1F1E8-1F1EC"},
    "shortcode": "flag-cg",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS CG",
    "category": "flag"
  },
  {
    "name": "flag-ch",
    "unicode": {"apple":"1F1E8-1F1ED", "google":"1F1E8-1F1ED", "twitter":"1F1E8-1F1ED"},
    "shortcode": "flag-ch",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS CH",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-ci",
    "unicode": {"apple":"1F1E8-1F1EE", "google":"1F1E8-1F1EE", "twitter":"1F1E8-1F1EE"},
    "shortcode": "flag-ci",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS CI",
    "category": "flag"
  },
  {
    "name": "flag-ck",
    "unicode": {"apple":"1F1E8-1F1F0", "google":"1F1E8-1F1F0", "twitter":"1F1E8-1F1F0"},
    "shortcode": "flag-ck",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS CK",
    "category": "flag"
  },
  {
    "name": "flag-cl",
    "unicode": {"apple":"1F1E8-1F1F1", "google":"1F1E8-1F1F1", "twitter":"1F1E8-1F1F1"},
    "shortcode": "flag-cl",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS CL",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-cm",
    "unicode": {"apple":"1F1E8-1F1F2", "google":"1F1E8-1F1F2", "twitter":"1F1E8-1F1F2"},
    "shortcode": "flag-cm",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS CM",
    "category": "flag"
  },
  {
    "name": "flag-cn",
    "unicode": {"apple":"1F1E8-1F1F3", "google":"1F1E8-1F1F3", "twitter":"1F1E8-1F1F3"},
    "shortcode": "flag-cn",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS CN",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-co",
    "unicode": {"apple":"1F1E8-1F1F4", "google":"1F1E8-1F1F4", "twitter":"1F1E8-1F1F4"},
    "shortcode": "flag-co",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS CO",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-cp",
    "unicode": {"apple":"1F1E8-1F1F5", "google":"1F1E8-1F1F5", "twitter":"1F1E8-1F1F5"},
    "shortcode": "flag-cp",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS CP",
    "category": "flag"
  },
  {
    "name": "flag-cr",
    "unicode": {"apple":"1F1E8-1F1F7", "google":"1F1E8-1F1F7", "twitter":"1F1E8-1F1F7"},
    "shortcode": "flag-cr",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS CR",
    "category": "flag"
  },
  {
    "name": "flag-cu",
    "unicode": {"apple":"1F1E8-1F1FA", "google":"1F1E8-1F1FA", "twitter":"1F1E8-1F1FA"},
    "shortcode": "flag-cu",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS CU",
    "category": "flag"
  },
  {
    "name": "flag-cv",
    "unicode": {"apple":"1F1E8-1F1FB", "google":"1F1E8-1F1FB", "twitter":"1F1E8-1F1FB"},
    "shortcode": "flag-cv",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS CV",
    "category": "flag"
  },
  {
    "name": "flag-cw",
    "unicode": {"apple":"1F1E8-1F1FC", "google":"1F1E8-1F1FC", "twitter":"1F1E8-1F1FC"},
    "shortcode": "flag-cw",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS CW",
    "category": "flag"
  },
  {
    "name": "flag-cx",
    "unicode": {"apple":"1F1E8-1F1FD", "google":"1F1E8-1F1FD", "twitter":"1F1E8-1F1FD"},
    "shortcode": "flag-cx",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS CX",
    "category": "flag"
  },
  {
    "name": "flag-cy",
    "unicode": {"apple":"1F1E8-1F1FE", "google":"1F1E8-1F1FE", "twitter":"1F1E8-1F1FE"},
    "shortcode": "flag-cy",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS CY",
    "category": "flag"
  },
  {
    "name": "flag-cz",
    "unicode": {"apple":"1F1E8-1F1FF", "google":"1F1E8-1F1FF", "twitter":"1F1E8-1F1FF"},
    "shortcode": "flag-cz",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS CZ",
    "category": "flag"
  },
  {
    "name": "flag-de",
    "unicode": {"apple":"1F1E9-1F1EA", "google":"1F1E9-1F1EA", "twitter":"1F1E9-1F1EA"},
    "shortcode": "flag-de",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS DE",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-dg",
    "unicode": {"apple":"1F1E9-1F1EC", "google":"1F1E9-1F1EC", "twitter":"1F1E9-1F1EC"},
    "shortcode": "flag-dg",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS DG",
    "category": "flag"
  },
  {
    "name": "flag-dj",
    "unicode": {"apple":"1F1E9-1F1EF", "google":"1F1E9-1F1EF", "twitter":"1F1E9-1F1EF"},
    "shortcode": "flag-dj",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS DJ",
    "category": "flag"
  },
  {
    "name": "flag-dk",
    "unicode": {"apple":"1F1E9-1F1F0", "google":"1F1E9-1F1F0", "twitter":"1F1E9-1F1F0"},
    "shortcode": "flag-dk",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS DK",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-dm",
    "unicode": {"apple":"1F1E9-1F1F2", "google":"1F1E9-1F1F2", "twitter":"1F1E9-1F1F2"},
    "shortcode": "flag-dm",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS DM",
    "category": "flag"
  },
  {
    "name": "flag-do",
    "unicode": {"apple":"1F1E9-1F1F4", "google":"1F1E9-1F1F4", "twitter":"1F1E9-1F1F4"},
    "shortcode": "flag-do",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS DO",
    "category": "flag"
  },
  {
    "name": "flag-dz",
    "unicode": {"apple":"1F1E9-1F1FF", "google":"1F1E9-1F1FF", "twitter":"1F1E9-1F1FF"},
    "shortcode": "flag-dz",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS DZ",
    "category": "flag"
  },
  {
    "name": "flag-ea",
    "unicode": {"apple":"1F1EA-1F1E6", "google":"1F1EA-1F1E6", "twitter":"1F1EA-1F1E6"},
    "shortcode": "flag-ea",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS EA",
    "category": "flag"
  },
  {
    "name": "flag-ec",
    "unicode": {"apple":"1F1EA-1F1E8", "google":"1F1EA-1F1E8", "twitter":"1F1EA-1F1E8"},
    "shortcode": "flag-ec",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS EC",
    "category": "flag"
  },
  {
    "name": "flag-ee",
    "unicode": {"apple":"1F1EA-1F1EA", "google":"1F1EA-1F1EA", "twitter":"1F1EA-1F1EA"},
    "shortcode": "flag-ee",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS EE",
    "category": "flag"
  },
  {
    "name": "flag-eg",
    "unicode": {"apple":"1F1EA-1F1EC", "google":"1F1EA-1F1EC", "twitter":"1F1EA-1F1EC"},
    "shortcode": "flag-eg",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS EG",
    "category": "flag"
  },
  {
    "name": "flag-eh",
    "unicode": {"apple":"1F1EA-1F1ED", "google":"1F1EA-1F1ED", "twitter":"1F1EA-1F1ED"},
    "shortcode": "flag-eh",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS EH",
    "category": "flag"
  },
  {
    "name": "flag-er",
    "unicode": {"apple":"1F1EA-1F1F7", "google":"1F1EA-1F1F7", "twitter":"1F1EA-1F1F7"},
    "shortcode": "flag-er",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS ER",
    "category": "flag"
  },
  {
    "name": "flag-es",
    "unicode": {"apple":"1F1EA-1F1F8", "google":"1F1EA-1F1F8", "twitter":"1F1EA-1F1F8"},
    "shortcode": "flag-es",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS ES",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-et",
    "unicode": {"apple":"1F1EA-1F1F9", "google":"1F1EA-1F1F9", "twitter":"1F1EA-1F1F9"},
    "shortcode": "flag-et",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS ET",
    "category": "flag"
  },
  {
    "name": "flag-eu",
    "unicode": {"apple":"1F1EA-1F1FA", "google":"1F1EA-1F1FA", "twitter":"1F1EA-1F1FA"},
    "shortcode": "flag-eu",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS EU",
    "category": "flag"
  },
  {
    "name": "flag-fi",
    "unicode": {"apple":"1F1EB-1F1EE", "google":"1F1EB-1F1EE", "twitter":"1F1EB-1F1EE"},
    "shortcode": "flag-fi",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS FI",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-fj",
    "unicode": {"apple":"1F1EB-1F1EF", "google":"1F1EB-1F1EF", "twitter":"1F1EB-1F1EF"},
    "shortcode": "flag-fj",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS FJ",
    "category": "flag"
  },
  {
    "name": "flag-fk",
    "unicode": {"apple":"1F1EB-1F1F0", "google":"1F1EB-1F1F0", "twitter":"1F1EB-1F1F0"},
    "shortcode": "flag-fk",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS FK",
    "category": "flag"
  },
  {
    "name": "flag-fm",
    "unicode": {"apple":"1F1EB-1F1F2", "google":"1F1EB-1F1F2", "twitter":"1F1EB-1F1F2"},
    "shortcode": "flag-fm",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS FM",
    "category": "flag"
  },
  {
    "name": "flag-fo",
    "unicode": {"apple":"1F1EB-1F1F4", "google":"1F1EB-1F1F4", "twitter":"1F1EB-1F1F4"},
    "shortcode": "flag-fo",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS FO",
    "category": "flag"
  },
  {
    "name": "flag-fr",
    "unicode": {"apple":"1F1EB-1F1F7", "google":"1F1EB-1F1F7", "twitter":"1F1EB-1F1F7"},
    "shortcode": "flag-fr",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS FR",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-ga",
    "unicode": {"apple":"1F1EC-1F1E6", "google":"1F1EC-1F1E6", "twitter":"1F1EC-1F1E6"},
    "shortcode": "flag-ga",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS GA",
    "category": "flag"
  },
  {
    "name": "flag-gb",
    "unicode": {"apple":"1F1EC-1F1E7", "google":"1F1EC-1F1E7", "twitter":"1F1EC-1F1E7"},
    "shortcode": "flag-gb",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS GB",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-gd",
    "unicode": {"apple":"1F1EC-1F1E9", "google":"1F1EC-1F1E9", "twitter":"1F1EC-1F1E9"},
    "shortcode": "flag-gd",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS GD",
    "category": "flag"
  },
  {
    "name": "flag-ge",
    "unicode": {"apple":"1F1EC-1F1EA", "google":"1F1EC-1F1EA", "twitter":"1F1EC-1F1EA"},
    "shortcode": "flag-ge",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS GE",
    "category": "flag"
  },
  {
    "name": "flag-gf",
    "unicode": {"apple":"1F1EC-1F1EB", "google":"1F1EC-1F1EB", "twitter":"1F1EC-1F1EB"},
    "shortcode": "flag-gf",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS GF",
    "category": "flag"
  },
  {
    "name": "flag-gg",
    "unicode": {"apple":"1F1EC-1F1EC", "google":"1F1EC-1F1EC", "twitter":"1F1EC-1F1EC"},
    "shortcode": "flag-gg",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS GG",
    "category": "flag"
  },
  {
    "name": "flag-gh",
    "unicode": {"apple":"1F1EC-1F1ED", "google":"1F1EC-1F1ED", "twitter":"1F1EC-1F1ED"},
    "shortcode": "flag-gh",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS GH",
    "category": "flag"
  },
  {
    "name": "flag-gi",
    "unicode": {"apple":"1F1EC-1F1EE", "google":"1F1EC-1F1EE", "twitter":"1F1EC-1F1EE"},
    "shortcode": "flag-gi",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS GI",
    "category": "flag"
  },
  {
    "name": "flag-gl",
    "unicode": {"apple":"1F1EC-1F1F1", "google":"1F1EC-1F1F1", "twitter":"1F1EC-1F1F1"},
    "shortcode": "flag-gl",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS GL",
    "category": "flag"
  },
  {
    "name": "flag-gm",
    "unicode": {"apple":"1F1EC-1F1F2", "google":"1F1EC-1F1F2", "twitter":"1F1EC-1F1F2"},
    "shortcode": "flag-gm",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS GM",
    "category": "flag"
  },
  {
    "name": "flag-gn",
    "unicode": {"apple":"1F1EC-1F1F3", "google":"1F1EC-1F1F3", "twitter":"1F1EC-1F1F3"},
    "shortcode": "flag-gn",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS GN",
    "category": "flag"
  },
  {
    "name": "flag-gp",
    "unicode": {"apple":"1F1EC-1F1F5", "google":"1F1EC-1F1F5", "twitter":"1F1EC-1F1F5"},
    "shortcode": "flag-gp",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS GP",
    "category": "flag"
  },
  {
    "name": "flag-gq",
    "unicode": {"apple":"1F1EC-1F1F6", "google":"1F1EC-1F1F6", "twitter":"1F1EC-1F1F6"},
    "shortcode": "flag-gq",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS GQ",
    "category": "flag"
  },
  {
    "name": "flag-gr",
    "unicode": {"apple":"1F1EC-1F1F7", "google":"1F1EC-1F1F7", "twitter":"1F1EC-1F1F7"},
    "shortcode": "flag-gr",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS GR",
    "category": "flag"
  },
  {
    "name": "flag-gs",
    "unicode": {"apple":"1F1EC-1F1F8", "google":"1F1EC-1F1F8", "twitter":"1F1EC-1F1F8"},
    "shortcode": "flag-gs",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS GS",
    "category": "flag"
  },
  {
    "name": "flag-gt",
    "unicode": {"apple":"1F1EC-1F1F9", "google":"1F1EC-1F1F9", "twitter":"1F1EC-1F1F9"},
    "shortcode": "flag-gt",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS GT",
    "category": "flag"
  },
  {
    "name": "flag-gu",
    "unicode": {"apple":"1F1EC-1F1FA", "google":"1F1EC-1F1FA", "twitter":"1F1EC-1F1FA"},
    "shortcode": "flag-gu",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS GU",
    "category": "flag"
  },
  {
    "name": "flag-gw",
    "unicode": {"apple":"1F1EC-1F1FC", "google":"1F1EC-1F1FC", "twitter":"1F1EC-1F1FC"},
    "shortcode": "flag-gw",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS GW",
    "category": "flag"
  },
  {
    "name": "flag-gy",
    "unicode": {"apple":"1F1EC-1F1FE", "google":"1F1EC-1F1FE", "twitter":"1F1EC-1F1FE"},
    "shortcode": "flag-gy",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS GY",
    "category": "flag"
  },
  {
    "name": "flag-hk",
    "unicode": {"apple":"1F1ED-1F1F0", "google":"1F1ED-1F1F0", "twitter":"1F1ED-1F1F0"},
    "shortcode": "flag-hk",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS HK",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-hm",
    "unicode": {"apple":"1F1ED-1F1F2", "google":"1F1ED-1F1F2", "twitter":"1F1ED-1F1F2"},
    "shortcode": "flag-hm",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS HM",
    "category": "flag"
  },
  {
    "name": "flag-hn",
    "unicode": {"apple":"1F1ED-1F1F3", "google":"1F1ED-1F1F3", "twitter":"1F1ED-1F1F3"},
    "shortcode": "flag-hn",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS HN",
    "category": "flag"
  },
  {
    "name": "flag-hr",
    "unicode": {"apple":"1F1ED-1F1F7", "google":"1F1ED-1F1F7", "twitter":"1F1ED-1F1F7"},
    "shortcode": "flag-hr",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS HR",
    "category": "flag"
  },
  {
    "name": "flag-ht",
    "unicode": {"apple":"1F1ED-1F1F9", "google":"1F1ED-1F1F9", "twitter":"1F1ED-1F1F9"},
    "shortcode": "flag-ht",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS HT",
    "category": "flag"
  },
  {
    "name": "flag-hu",
    "unicode": {"apple":"1F1ED-1F1FA", "google":"1F1ED-1F1FA", "twitter":"1F1ED-1F1FA"},
    "shortcode": "flag-hu",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS HU",
    "category": "flag"
  },
  {
    "name": "flag-ic",
    "unicode": {"apple":"1F1EE-1F1E8", "google":"1F1EE-1F1E8", "twitter":"1F1EE-1F1E8"},
    "shortcode": "flag-ic",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS IC",
    "category": "flag"
  },
  {
    "name": "flag-id",
    "unicode": {"apple":"1F1EE-1F1E9", "google":"1F1EE-1F1E9", "twitter":"1F1EE-1F1E9"},
    "shortcode": "flag-id",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS ID",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-ie",
    "unicode": {"apple":"1F1EE-1F1EA", "google":"1F1EE-1F1EA", "twitter":"1F1EE-1F1EA"},
    "shortcode": "flag-ie",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS IE",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-il",
    "unicode": {"apple":"1F1EE-1F1F1", "google":"1F1EE-1F1F1", "twitter":"1F1EE-1F1F1"},
    "shortcode": "flag-il",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS IL",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-im",
    "unicode": {"apple":"1F1EE-1F1F2", "google":"1F1EE-1F1F2", "twitter":"1F1EE-1F1F2"},
    "shortcode": "flag-im",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS IM",
    "category": "flag"
  },
  {
    "name": "flag-in",
    "unicode": {"apple":"1F1EE-1F1F3", "google":"1F1EE-1F1F3", "twitter":"1F1EE-1F1F3"},
    "shortcode": "flag-in",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS IN",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-io",
    "unicode": {"apple":"1F1EE-1F1F4", "google":"1F1EE-1F1F4", "twitter":"1F1EE-1F1F4"},
    "shortcode": "flag-io",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS IO",
    "category": "flag"
  },
  {
    "name": "flag-iq",
    "unicode": {"apple":"1F1EE-1F1F6", "google":"1F1EE-1F1F6", "twitter":"1F1EE-1F1F6"},
    "shortcode": "flag-iq",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS IQ",
    "category": "flag"
  },
  {
    "name": "flag-ir",
    "unicode": {"apple":"1F1EE-1F1F7", "google":"1F1EE-1F1F7", "twitter":"1F1EE-1F1F7"},
    "shortcode": "flag-ir",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS IR",
    "category": "flag"
  },
  {
    "name": "flag-is",
    "unicode": {"apple":"1F1EE-1F1F8", "google":"1F1EE-1F1F8", "twitter":"1F1EE-1F1F8"},
    "shortcode": "flag-is",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS IS",
    "category": "flag"
  },
  {
    "name": "flag-it",
    "unicode": {"apple":"1F1EE-1F1F9", "google":"1F1EE-1F1F9", "twitter":"1F1EE-1F1F9"},
    "shortcode": "flag-it",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS IT",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-je",
    "unicode": {"apple":"1F1EF-1F1EA", "google":"1F1EF-1F1EA", "twitter":"1F1EF-1F1EA"},
    "shortcode": "flag-je",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS JE",
    "category": "flag"
  },
  {
    "name": "flag-jm",
    "unicode": {"apple":"1F1EF-1F1F2", "google":"1F1EF-1F1F2", "twitter":"1F1EF-1F1F2"},
    "shortcode": "flag-jm",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS JM",
    "category": "flag"
  },
  {
    "name": "flag-jo",
    "unicode": {"apple":"1F1EF-1F1F4", "google":"1F1EF-1F1F4", "twitter":"1F1EF-1F1F4"},
    "shortcode": "flag-jo",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS JO",
    "category": "flag"
  },
  {
    "name": "flag-jp",
    "unicode": {"apple":"1F1EF-1F1F5", "google":"1F1EF-1F1F5", "twitter":"1F1EF-1F1F5"},
    "shortcode": "flag-jp",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS JP",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-ke",
    "unicode": {"apple":"1F1F0-1F1EA", "google":"1F1F0-1F1EA", "twitter":"1F1F0-1F1EA"},
    "shortcode": "flag-ke",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS KE",
    "category": "flag"
  },
  {
    "name": "flag-kg",
    "unicode": {"apple":"1F1F0-1F1EC", "google":"1F1F0-1F1EC", "twitter":"1F1F0-1F1EC"},
    "shortcode": "flag-kg",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS KG",
    "category": "flag"
  },
  {
    "name": "flag-kh",
    "unicode": {"apple":"1F1F0-1F1ED", "google":"1F1F0-1F1ED", "twitter":"1F1F0-1F1ED"},
    "shortcode": "flag-kh",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS KH",
    "category": "flag"
  },
  {
    "name": "flag-ki",
    "unicode": {"apple":"1F1F0-1F1EE", "google":"1F1F0-1F1EE", "twitter":"1F1F0-1F1EE"},
    "shortcode": "flag-ki",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS KI",
    "category": "flag"
  },
  {
    "name": "flag-km",
    "unicode": {"apple":"1F1F0-1F1F2", "google":"1F1F0-1F1F2", "twitter":"1F1F0-1F1F2"},
    "shortcode": "flag-km",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS KM",
    "category": "flag"
  },
  {
    "name": "flag-kn",
    "unicode": {"apple":"1F1F0-1F1F3", "google":"1F1F0-1F1F3", "twitter":"1F1F0-1F1F3"},
    "shortcode": "flag-kn",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS KN",
    "category": "flag"
  },
  {
    "name": "flag-kp",
    "unicode": {"apple":"1F1F0-1F1F5", "google":"1F1F0-1F1F5", "twitter":"1F1F0-1F1F5"},
    "shortcode": "flag-kp",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS KP",
    "category": "flag"
  },
  {
    "name": "flag-kr",
    "unicode": {"apple":"1F1F0-1F1F7", "google":"1F1F0-1F1F7", "twitter":"1F1F0-1F1F7"},
    "shortcode": "flag-kr",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS KR",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-kw",
    "unicode": {"apple":"1F1F0-1F1FC", "google":"1F1F0-1F1FC", "twitter":"1F1F0-1F1FC"},
    "shortcode": "flag-kw",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS KW",
    "category": "flag"
  },
  {
    "name": "flag-ky",
    "unicode": {"apple":"1F1F0-1F1FE", "google":"1F1F0-1F1FE", "twitter":"1F1F0-1F1FE"},
    "shortcode": "flag-ky",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS KY",
    "category": "flag"
  },
  {
    "name": "flag-kz",
    "unicode": {"apple":"1F1F0-1F1FF", "google":"1F1F0-1F1FF", "twitter":"1F1F0-1F1FF"},
    "shortcode": "flag-kz",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS KZ",
    "category": "flag"
  },
  {
    "name": "flag-la",
    "unicode": {"apple":"1F1F1-1F1E6", "google":"1F1F1-1F1E6", "twitter":"1F1F1-1F1E6"},
    "shortcode": "flag-la",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS LA",
    "category": "flag"
  },
  {
    "name": "flag-lb",
    "unicode": {"apple":"1F1F1-1F1E7", "google":"1F1F1-1F1E7", "twitter":"1F1F1-1F1E7"},
    "shortcode": "flag-lb",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS LB",
    "category": "flag"
  },
  {
    "name": "flag-lc",
    "unicode": {"apple":"1F1F1-1F1E8", "google":"1F1F1-1F1E8", "twitter":"1F1F1-1F1E8"},
    "shortcode": "flag-lc",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS LC",
    "category": "flag"
  },
  {
    "name": "flag-li",
    "unicode": {"apple":"1F1F1-1F1EE", "google":"1F1F1-1F1EE", "twitter":"1F1F1-1F1EE"},
    "shortcode": "flag-li",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS LI",
    "category": "flag"
  },
  {
    "name": "flag-lk",
    "unicode": {"apple":"1F1F1-1F1F0", "google":"1F1F1-1F1F0", "twitter":"1F1F1-1F1F0"},
    "shortcode": "flag-lk",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS LK",
    "category": "flag"
  },
  {
    "name": "flag-lr",
    "unicode": {"apple":"1F1F1-1F1F7", "google":"1F1F1-1F1F7", "twitter":"1F1F1-1F1F7"},
    "shortcode": "flag-lr",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS LR",
    "category": "flag"
  },
  {
    "name": "flag-ls",
    "unicode": {"apple":"1F1F1-1F1F8", "google":"1F1F1-1F1F8", "twitter":"1F1F1-1F1F8"},
    "shortcode": "flag-ls",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS LS",
    "category": "flag"
  },
  {
    "name": "flag-lt",
    "unicode": {"apple":"1F1F1-1F1F9", "google":"1F1F1-1F1F9", "twitter":"1F1F1-1F1F9"},
    "shortcode": "flag-lt",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS LT",
    "category": "flag"
  },
  {
    "name": "flag-lu",
    "unicode": {"apple":"1F1F1-1F1FA", "google":"1F1F1-1F1FA", "twitter":"1F1F1-1F1FA"},
    "shortcode": "flag-lu",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS LU",
    "category": "flag"
  },
  {
    "name": "flag-lv",
    "unicode": {"apple":"1F1F1-1F1FB", "google":"1F1F1-1F1FB", "twitter":"1F1F1-1F1FB"},
    "shortcode": "flag-lv",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS LV",
    "category": "flag"
  },
  {
    "name": "flag-ly",
    "unicode": {"apple":"1F1F1-1F1FE", "google":"1F1F1-1F1FE", "twitter":"1F1F1-1F1FE"},
    "shortcode": "flag-ly",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS LY",
    "category": "flag"
  },
  {
    "name": "flag-ma",
    "unicode": {"apple":"1F1F2-1F1E6", "google":"1F1F2-1F1E6", "twitter":"1F1F2-1F1E6"},
    "shortcode": "flag-ma",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS MA",
    "category": "flag"
  },
  {
    "name": "flag-mc",
    "unicode": {"apple":"1F1F2-1F1E8", "google":"1F1F2-1F1E8", "twitter":"1F1F2-1F1E8"},
    "shortcode": "flag-mc",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS MC",
    "category": "flag"
  },
  {
    "name": "flag-md",
    "unicode": {"apple":"1F1F2-1F1E9", "google":"1F1F2-1F1E9", "twitter":"1F1F2-1F1E9"},
    "shortcode": "flag-md",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS MD",
    "category": "flag"
  },
  {
    "name": "flag-me",
    "unicode": {"apple":"1F1F2-1F1EA", "google":"1F1F2-1F1EA", "twitter":"1F1F2-1F1EA"},
    "shortcode": "flag-me",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS ME",
    "category": "flag"
  },
  {
    "name": "flag-mf",
    "unicode": {"apple":"1F1F2-1F1EB", "google":"1F1F2-1F1EB", "twitter":"1F1F2-1F1EB"},
    "shortcode": "flag-mf",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS MF",
    "category": "flag"
  },
  {
    "name": "flag-mg",
    "unicode": {"apple":"1F1F2-1F1EC", "google":"1F1F2-1F1EC", "twitter":"1F1F2-1F1EC"},
    "shortcode": "flag-mg",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS MG",
    "category": "flag"
  },
  {
    "name": "flag-mh",
    "unicode": {"apple":"1F1F2-1F1ED", "google":"1F1F2-1F1ED", "twitter":"1F1F2-1F1ED"},
    "shortcode": "flag-mh",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS MH",
    "category": "flag"
  },
  {
    "name": "flag-mk",
    "unicode": {"apple":"1F1F2-1F1F0", "google":"1F1F2-1F1F0", "twitter":"1F1F2-1F1F0"},
    "shortcode": "flag-mk",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS MK",
    "category": "flag"
  },
  {
    "name": "flag-ml",
    "unicode": {"apple":"1F1F2-1F1F1", "google":"1F1F2-1F1F1", "twitter":"1F1F2-1F1F1"},
    "shortcode": "flag-ml",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS ML",
    "category": "flag"
  },
  {
    "name": "flag-mm",
    "unicode": {"apple":"1F1F2-1F1F2", "google":"1F1F2-1F1F2", "twitter":"1F1F2-1F1F2"},
    "shortcode": "flag-mm",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS MM",
    "category": "flag"
  },
  {
    "name": "flag-mn",
    "unicode": {"apple":"1F1F2-1F1F3", "google":"1F1F2-1F1F3", "twitter":"1F1F2-1F1F3"},
    "shortcode": "flag-mn",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS MN",
    "category": "flag"
  },
  {
    "name": "flag-mo",
    "unicode": {"apple":"1F1F2-1F1F4", "google":"1F1F2-1F1F4", "twitter":"1F1F2-1F1F4"},
    "shortcode": "flag-mo",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS MO",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-mp",
    "unicode": {"apple":"1F1F2-1F1F5", "google":"1F1F2-1F1F5", "twitter":"1F1F2-1F1F5"},
    "shortcode": "flag-mp",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS MP",
    "category": "flag"
  },
  {
    "name": "flag-mq",
    "unicode": {"apple":"1F1F2-1F1F6", "google":"1F1F2-1F1F6", "twitter":"1F1F2-1F1F6"},
    "shortcode": "flag-mq",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS MQ",
    "category": "flag"
  },
  {
    "name": "flag-mr",
    "unicode": {"apple":"1F1F2-1F1F7", "google":"1F1F2-1F1F7", "twitter":"1F1F2-1F1F7"},
    "shortcode": "flag-mr",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS MR",
    "category": "flag"
  },
  {
    "name": "flag-ms",
    "unicode": {"apple":"1F1F2-1F1F8", "google":"1F1F2-1F1F8", "twitter":"1F1F2-1F1F8"},
    "shortcode": "flag-ms",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS MS",
    "category": "flag"
  },
  {
    "name": "flag-mt",
    "unicode": {"apple":"1F1F2-1F1F9", "google":"1F1F2-1F1F9", "twitter":"1F1F2-1F1F9"},
    "shortcode": "flag-mt",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS MT",
    "category": "flag"
  },
  {
    "name": "flag-mu",
    "unicode": {"apple":"1F1F2-1F1FA", "google":"1F1F2-1F1FA", "twitter":"1F1F2-1F1FA"},
    "shortcode": "flag-mu",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS MU",
    "category": "flag"
  },
  {
    "name": "flag-mv",
    "unicode": {"apple":"1F1F2-1F1FB", "google":"1F1F2-1F1FB", "twitter":"1F1F2-1F1FB"},
    "shortcode": "flag-mv",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS MV",
    "category": "flag"
  },
  {
    "name": "flag-mw",
    "unicode": {"apple":"1F1F2-1F1FC", "google":"1F1F2-1F1FC", "twitter":"1F1F2-1F1FC"},
    "shortcode": "flag-mw",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS MW",
    "category": "flag"
  },
  {
    "name": "flag-mx",
    "unicode": {"apple":"1F1F2-1F1FD", "google":"1F1F2-1F1FD", "twitter":"1F1F2-1F1FD"},
    "shortcode": "flag-mx",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS MX",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-my",
    "unicode": {"apple":"1F1F2-1F1FE", "google":"1F1F2-1F1FE", "twitter":"1F1F2-1F1FE"},
    "shortcode": "flag-my",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS MY",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-mz",
    "unicode": {"apple":"1F1F2-1F1FF", "google":"1F1F2-1F1FF", "twitter":"1F1F2-1F1FF"},
    "shortcode": "flag-mz",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS MZ",
    "category": "flag"
  },
  {
    "name": "flag-na",
    "unicode": {"apple":"1F1F3-1F1E6", "google":"1F1F3-1F1E6", "twitter":"1F1F3-1F1E6"},
    "shortcode": "flag-na",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS NA",
    "category": "flag"
  },
  {
    "name": "flag-nc",
    "unicode": {"apple":"1F1F3-1F1E8", "google":"1F1F3-1F1E8", "twitter":"1F1F3-1F1E8"},
    "shortcode": "flag-nc",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS NC",
    "category": "flag"
  },
  {
    "name": "flag-ne",
    "unicode": {"apple":"1F1F3-1F1EA", "google":"1F1F3-1F1EA", "twitter":"1F1F3-1F1EA"},
    "shortcode": "flag-ne",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS NE",
    "category": "flag"
  },
  {
    "name": "flag-nf",
    "unicode": {"apple":"1F1F3-1F1EB", "google":"1F1F3-1F1EB", "twitter":"1F1F3-1F1EB"},
    "shortcode": "flag-nf",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS NF",
    "category": "flag"
  },
  {
    "name": "flag-ng",
    "unicode": {"apple":"1F1F3-1F1EC", "google":"1F1F3-1F1EC", "twitter":"1F1F3-1F1EC"},
    "shortcode": "flag-ng",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS NG",
    "category": "flag"
  },
  {
    "name": "flag-ni",
    "unicode": {"apple":"1F1F3-1F1EE", "google":"1F1F3-1F1EE", "twitter":"1F1F3-1F1EE"},
    "shortcode": "flag-ni",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS NI",
    "category": "flag"
  },
  {
    "name": "flag-nl",
    "unicode": {"apple":"1F1F3-1F1F1", "google":"1F1F3-1F1F1", "twitter":"1F1F3-1F1F1"},
    "shortcode": "flag-nl",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS NL",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-no",
    "unicode": {"apple":"1F1F3-1F1F4", "google":"1F1F3-1F1F4", "twitter":"1F1F3-1F1F4"},
    "shortcode": "flag-no",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS NO",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-np",
    "unicode": {"apple":"1F1F3-1F1F5", "google":"1F1F3-1F1F5", "twitter":"1F1F3-1F1F5"},
    "shortcode": "flag-np",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS NP",
    "category": "flag"
  },
  {
    "name": "flag-nr",
    "unicode": {"apple":"1F1F3-1F1F7", "google":"1F1F3-1F1F7", "twitter":"1F1F3-1F1F7"},
    "shortcode": "flag-nr",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS NR",
    "category": "flag"
  },
  {
    "name": "flag-nu",
    "unicode": {"apple":"1F1F3-1F1FA", "google":"1F1F3-1F1FA", "twitter":"1F1F3-1F1FA"},
    "shortcode": "flag-nu",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS NU",
    "category": "flag"
  },
  {
    "name": "flag-nz",
    "unicode": {"apple":"1F1F3-1F1FF", "google":"1F1F3-1F1FF", "twitter":"1F1F3-1F1FF"},
    "shortcode": "flag-nz",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS NZ",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-om",
    "unicode": {"apple":"1F1F4-1F1F2", "google":"1F1F4-1F1F2", "twitter":"1F1F4-1F1F2"},
    "shortcode": "flag-om",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS OM",
    "category": "flag"
  },
  {
    "name": "flag-pa",
    "unicode": {"apple":"1F1F5-1F1E6", "google":"1F1F5-1F1E6", "twitter":"1F1F5-1F1E6"},
    "shortcode": "flag-pa",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS PA",
    "category": "flag"
  },
  {
    "name": "flag-pe",
    "unicode": {"apple":"1F1F5-1F1EA", "google":"1F1F5-1F1EA", "twitter":"1F1F5-1F1EA"},
    "shortcode": "flag-pe",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS PE",
    "category": "flag"
  },
  {
    "name": "flag-pf",
    "unicode": {"apple":"1F1F5-1F1EB", "google":"1F1F5-1F1EB", "twitter":"1F1F5-1F1EB"},
    "shortcode": "flag-pf",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS PF",
    "category": "flag"
  },
  {
    "name": "flag-pg",
    "unicode": {"apple":"1F1F5-1F1EC", "google":"1F1F5-1F1EC", "twitter":"1F1F5-1F1EC"},
    "shortcode": "flag-pg",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS PG",
    "category": "flag"
  },
  {
    "name": "flag-ph",
    "unicode": {"apple":"1F1F5-1F1ED", "google":"1F1F5-1F1ED", "twitter":"1F1F5-1F1ED"},
    "shortcode": "flag-ph",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS PH",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-pk",
    "unicode": {"apple":"1F1F5-1F1F0", "google":"1F1F5-1F1F0", "twitter":"1F1F5-1F1F0"},
    "shortcode": "flag-pk",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS PK",
    "category": "flag"
  },
  {
    "name": "flag-pl",
    "unicode": {"apple":"1F1F5-1F1F1", "google":"1F1F5-1F1F1", "twitter":"1F1F5-1F1F1"},
    "shortcode": "flag-pl",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS PL",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-pm",
    "unicode": {"apple":"1F1F5-1F1F2", "google":"1F1F5-1F1F2", "twitter":"1F1F5-1F1F2"},
    "shortcode": "flag-pm",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS PM",
    "category": "flag"
  },
  {
    "name": "flag-pn",
    "unicode": {"apple":"1F1F5-1F1F3", "google":"1F1F5-1F1F3", "twitter":"1F1F5-1F1F3"},
    "shortcode": "flag-pn",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS PN",
    "category": "flag"
  },
  {
    "name": "flag-pr",
    "unicode": {"apple":"1F1F5-1F1F7", "google":"1F1F5-1F1F7", "twitter":"1F1F5-1F1F7"},
    "shortcode": "flag-pr",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS PR",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-ps",
    "unicode": {"apple":"1F1F5-1F1F8", "google":"1F1F5-1F1F8", "twitter":"1F1F5-1F1F8"},
    "shortcode": "flag-ps",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS PS",
    "category": "flag"
  },
  {
    "name": "flag-pt",
    "unicode": {"apple":"1F1F5-1F1F9", "google":"1F1F5-1F1F9", "twitter":"1F1F5-1F1F9"},
    "shortcode": "flag-pt",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS PT",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-pw",
    "unicode": {"apple":"1F1F5-1F1FC", "google":"1F1F5-1F1FC", "twitter":"1F1F5-1F1FC"},
    "shortcode": "flag-pw",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS PW",
    "category": "flag"
  },
  {
    "name": "flag-py",
    "unicode": {"apple":"1F1F5-1F1FE", "google":"1F1F5-1F1FE", "twitter":"1F1F5-1F1FE"},
    "shortcode": "flag-py",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS PY",
    "category": "flag"
  },
  {
    "name": "flag-qa",
    "unicode": {"apple":"1F1F6-1F1E6", "google":"1F1F6-1F1E6", "twitter":"1F1F6-1F1E6"},
    "shortcode": "flag-qa",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS QA",
    "category": "flag"
  },
  {
    "name": "flag-re",
    "unicode": {"apple":"1F1F7-1F1EA", "google":"1F1F7-1F1EA", "twitter":"1F1F7-1F1EA"},
    "shortcode": "flag-re",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS RE",
    "category": "flag"
  },
  {
    "name": "flag-ro",
    "unicode": {"apple":"1F1F7-1F1F4", "google":"1F1F7-1F1F4", "twitter":"1F1F7-1F1F4"},
    "shortcode": "flag-ro",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS RO",
    "category": "flag"
  },
  {
    "name": "flag-rs",
    "unicode": {"apple":"1F1F7-1F1F8", "google":"1F1F7-1F1F8", "twitter":"1F1F7-1F1F8"},
    "shortcode": "flag-rs",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS RS",
    "category": "flag"
  },
  {
    "name": "flag-ru",
    "unicode": {"apple":"1F1F7-1F1FA", "google":"1F1F7-1F1FA", "twitter":"1F1F7-1F1FA"},
    "shortcode": "flag-ru",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS RU",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-rw",
    "unicode": {"apple":"1F1F7-1F1FC", "google":"1F1F7-1F1FC", "twitter":"1F1F7-1F1FC"},
    "shortcode": "flag-rw",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS RW",
    "category": "flag"
  },
  {
    "name": "flag-sa",
    "unicode": {"apple":"1F1F8-1F1E6", "google":"1F1F8-1F1E6", "twitter":"1F1F8-1F1E6"},
    "shortcode": "flag-sa",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS SA",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-sb",
    "unicode": {"apple":"1F1F8-1F1E7", "google":"1F1F8-1F1E7", "twitter":"1F1F8-1F1E7"},
    "shortcode": "flag-sb",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS SB",
    "category": "flag"
  },
  {
    "name": "flag-sc",
    "unicode": {"apple":"1F1F8-1F1E8", "google":"1F1F8-1F1E8", "twitter":"1F1F8-1F1E8"},
    "shortcode": "flag-sc",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS SC",
    "category": "flag"
  },
  {
    "name": "flag-sd",
    "unicode": {"apple":"1F1F8-1F1E9", "google":"1F1F8-1F1E9", "twitter":"1F1F8-1F1E9"},
    "shortcode": "flag-sd",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS SD",
    "category": "flag"
  },
  {
    "name": "flag-se",
    "unicode": {"apple":"1F1F8-1F1EA", "google":"1F1F8-1F1EA", "twitter":"1F1F8-1F1EA"},
    "shortcode": "flag-se",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS SE",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-sg",
    "unicode": {"apple":"1F1F8-1F1EC", "google":"1F1F8-1F1EC", "twitter":"1F1F8-1F1EC"},
    "shortcode": "flag-sg",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS SG",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-sh",
    "unicode": {"apple":"1F1F8-1F1ED", "google":"1F1F8-1F1ED", "twitter":"1F1F8-1F1ED"},
    "shortcode": "flag-sh",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS SH",
    "category": "flag"
  },
  {
    "name": "flag-si",
    "unicode": {"apple":"1F1F8-1F1EE", "google":"1F1F8-1F1EE", "twitter":"1F1F8-1F1EE"},
    "shortcode": "flag-si",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS SI",
    "category": "flag"
  },
  {
    "name": "flag-sj",
    "unicode": {"apple":"1F1F8-1F1EF", "google":"1F1F8-1F1EF", "twitter":"1F1F8-1F1EF"},
    "shortcode": "flag-sj",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS SJ",
    "category": "flag"
  },
  {
    "name": "flag-sk",
    "unicode": {"apple":"1F1F8-1F1F0", "google":"1F1F8-1F1F0", "twitter":"1F1F8-1F1F0"},
    "shortcode": "flag-sk",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS SK",
    "category": "flag"
  },
  {
    "name": "flag-sl",
    "unicode": {"apple":"1F1F8-1F1F1", "google":"1F1F8-1F1F1", "twitter":"1F1F8-1F1F1"},
    "shortcode": "flag-sl",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS SL",
    "category": "flag"
  },
  {
    "name": "flag-sm",
    "unicode": {"apple":"1F1F8-1F1F2", "google":"1F1F8-1F1F2", "twitter":"1F1F8-1F1F2"},
    "shortcode": "flag-sm",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS SM",
    "category": "flag"
  },
  {
    "name": "flag-sn",
    "unicode": {"apple":"1F1F8-1F1F3", "google":"1F1F8-1F1F3", "twitter":"1F1F8-1F1F3"},
    "shortcode": "flag-sn",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS SN",
    "category": "flag"
  },
  {
    "name": "flag-so",
    "unicode": {"apple":"1F1F8-1F1F4", "google":"1F1F8-1F1F4", "twitter":"1F1F8-1F1F4"},
    "shortcode": "flag-so",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS SO",
    "category": "flag"
  },
  {
    "name": "flag-sr",
    "unicode": {"apple":"1F1F8-1F1F7", "google":"1F1F8-1F1F7", "twitter":"1F1F8-1F1F7"},
    "shortcode": "flag-sr",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS SR",
    "category": "flag"
  },
  {
    "name": "flag-ss",
    "unicode": {"apple":"1F1F8-1F1F8", "google":"1F1F8-1F1F8", "twitter":"1F1F8-1F1F8"},
    "shortcode": "flag-ss",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS SS",
    "category": "flag"
  },
  {
    "name": "flag-st",
    "unicode": {"apple":"1F1F8-1F1F9", "google":"1F1F8-1F1F9", "twitter":"1F1F8-1F1F9"},
    "shortcode": "flag-st",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS ST",
    "category": "flag"
  },
  {
    "name": "flag-sv",
    "unicode": {"apple":"1F1F8-1F1FB", "google":"1F1F8-1F1FB", "twitter":"1F1F8-1F1FB"},
    "shortcode": "flag-sv",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS SV",
    "category": "flag"
  },
  {
    "name": "flag-sx",
    "unicode": {"apple":"1F1F8-1F1FD", "google":"1F1F8-1F1FD", "twitter":"1F1F8-1F1FD"},
    "shortcode": "flag-sx",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS SX",
    "category": "flag"
  },
  {
    "name": "flag-sy",
    "unicode": {"apple":"1F1F8-1F1FE", "google":"1F1F8-1F1FE", "twitter":"1F1F8-1F1FE"},
    "shortcode": "flag-sy",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS SY",
    "category": "flag"
  },
  {
    "name": "flag-sz",
    "unicode": {"apple":"1F1F8-1F1FF", "google":"1F1F8-1F1FF", "twitter":"1F1F8-1F1FF"},
    "shortcode": "flag-sz",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS SZ",
    "category": "flag"
  },
  {
    "name": "flag-ta",
    "unicode": {"apple":"1F1F9-1F1E6", "google":"1F1F9-1F1E6", "twitter":"1F1F9-1F1E6"},
    "shortcode": "flag-ta",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS TA",
    "category": "flag"
  },
  {
    "name": "flag-tc",
    "unicode": {"apple":"1F1F9-1F1E8", "google":"1F1F9-1F1E8", "twitter":"1F1F9-1F1E8"},
    "shortcode": "flag-tc",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS TC",
    "category": "flag"
  },
  {
    "name": "flag-td",
    "unicode": {"apple":"1F1F9-1F1E9", "google":"1F1F9-1F1E9", "twitter":"1F1F9-1F1E9"},
    "shortcode": "flag-td",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS TD",
    "category": "flag"
  },
  {
    "name": "flag-tf",
    "unicode": {"apple":"1F1F9-1F1EB", "google":"1F1F9-1F1EB", "twitter":"1F1F9-1F1EB"},
    "shortcode": "flag-tf",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS TF",
    "category": "flag"
  },
  {
    "name": "flag-tg",
    "unicode": {"apple":"1F1F9-1F1EC", "google":"1F1F9-1F1EC", "twitter":"1F1F9-1F1EC"},
    "shortcode": "flag-tg",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS TG",
    "category": "flag"
  },
  {
    "name": "flag-th",
    "unicode": {"apple":"1F1F9-1F1ED", "google":"1F1F9-1F1ED", "twitter":"1F1F9-1F1ED"},
    "shortcode": "flag-th",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS TH",
    "category": "flag"
  },
  {
    "name": "flag-tj",
    "unicode": {"apple":"1F1F9-1F1EF", "google":"1F1F9-1F1EF", "twitter":"1F1F9-1F1EF"},
    "shortcode": "flag-tj",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS TJ",
    "category": "flag"
  },
  {
    "name": "flag-tk",
    "unicode": {"apple":"1F1F9-1F1F0", "google":"1F1F9-1F1F0", "twitter":"1F1F9-1F1F0"},
    "shortcode": "flag-tk",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS TK",
    "category": "flag"
  },
  {
    "name": "flag-tl",
    "unicode": {"apple":"1F1F9-1F1F1", "google":"1F1F9-1F1F1", "twitter":"1F1F9-1F1F1"},
    "shortcode": "flag-tl",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS TL",
    "category": "flag"
  },
  {
    "name": "flag-tm",
    "unicode": {"apple":"1F1F9-1F1F2", "google":"1F1F9-1F1F2", "twitter":"1F1F9-1F1F2"},
    "shortcode": "flag-tm",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS TM",
    "category": "flag"
  },
  {
    "name": "flag-tn",
    "unicode": {"apple":"1F1F9-1F1F3", "google":"1F1F9-1F1F3", "twitter":"1F1F9-1F1F3"},
    "shortcode": "flag-tn",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS TN",
    "category": "flag"
  },
  {
    "name": "flag-to",
    "unicode": {"apple":"1F1F9-1F1F4", "google":"1F1F9-1F1F4", "twitter":"1F1F9-1F1F4"},
    "shortcode": "flag-to",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS TO",
    "category": "flag"
  },
  {
    "name": "flag-tr",
    "unicode": {"apple":"1F1F9-1F1F7", "google":"1F1F9-1F1F7", "twitter":"1F1F9-1F1F7"},
    "shortcode": "flag-tr",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS TR",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-tt",
    "unicode": {"apple":"1F1F9-1F1F9", "google":"1F1F9-1F1F9", "twitter":"1F1F9-1F1F9"},
    "shortcode": "flag-tt",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS TT",
    "category": "flag"
  },
  {
    "name": "flag-tv",
    "unicode": {"apple":"1F1F9-1F1FB", "google":"1F1F9-1F1FB", "twitter":"1F1F9-1F1FB"},
    "shortcode": "flag-tv",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS TV",
    "category": "flag"
  },
  {
    "name": "flag-tw",
    "unicode": {"apple":"1F1F9-1F1FC", "google":"1F1F9-1F1FC", "twitter":"1F1F9-1F1FC"},
    "shortcode": "flag-tw",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS TW",
    "category": "flag"
  },
  {
    "name": "flag-tz",
    "unicode": {"apple":"1F1F9-1F1FF", "google":"1F1F9-1F1FF", "twitter":"1F1F9-1F1FF"},
    "shortcode": "flag-tz",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS TZ",
    "category": "flag"
  },
  {
    "name": "flag-ua",
    "unicode": {"apple":"1F1FA-1F1E6", "google":"1F1FA-1F1E6", "twitter":"1F1FA-1F1E6"},
    "shortcode": "flag-ua",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS UA",
    "category": "flag"
  },
  {
    "name": "flag-ug",
    "unicode": {"apple":"1F1FA-1F1EC", "google":"1F1FA-1F1EC", "twitter":"1F1FA-1F1EC"},
    "shortcode": "flag-ug",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS UG",
    "category": "flag"
  },
  {
    "name": "flag-um",
    "unicode": {"apple":"1F1FA-1F1F2", "google":"1F1FA-1F1F2", "twitter":"1F1FA-1F1F2"},
    "shortcode": "flag-um",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS UM",
    "category": "flag"
  },
  {
    "name": "flag-us",
    "unicode": {"apple":"1F1FA-1F1F8", "google":"1F1FA-1F1F8", "twitter":"1F1FA-1F1F8"},
    "shortcode": "flag-us",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS US",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-uy",
    "unicode": {"apple":"1F1FA-1F1FE", "google":"1F1FA-1F1FE", "twitter":"1F1FA-1F1FE"},
    "shortcode": "flag-uy",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS UY",
    "category": "flag"
  },
  {
    "name": "flag-uz",
    "unicode": {"apple":"1F1FA-1F1FF", "google":"1F1FA-1F1FF", "twitter":"1F1FA-1F1FF"},
    "shortcode": "flag-uz",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS UZ",
    "category": "flag"
  },
  {
    "name": "flag-va",
    "unicode": {"apple":"1F1FB-1F1E6", "google":"1F1FB-1F1E6", "twitter":"1F1FB-1F1E6"},
    "shortcode": "flag-va",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS VA",
    "category": "flag"
  },
  {
    "name": "flag-vc",
    "unicode": {"apple":"1F1FB-1F1E8", "google":"1F1FB-1F1E8", "twitter":"1F1FB-1F1E8"},
    "shortcode": "flag-vc",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS VC",
    "category": "flag"
  },
  {
    "name": "flag-ve",
    "unicode": {"apple":"1F1FB-1F1EA", "google":"1F1FB-1F1EA", "twitter":"1F1FB-1F1EA"},
    "shortcode": "flag-ve",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS VE",
    "category": "flag"
  },
  {
    "name": "flag-vg",
    "unicode": {"apple":"1F1FB-1F1EC", "google":"1F1FB-1F1EC", "twitter":"1F1FB-1F1EC"},
    "shortcode": "flag-vg",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS VG",
    "category": "flag"
  },
  {
    "name": "flag-vi",
    "unicode": {"apple":"1F1FB-1F1EE", "google":"1F1FB-1F1EE", "twitter":"1F1FB-1F1EE"},
    "shortcode": "flag-vi",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS VI",
    "category": "flag"
  },
  {
    "name": "flag-vn",
    "unicode": {"apple":"1F1FB-1F1F3", "google":"1F1FB-1F1F3", "twitter":"1F1FB-1F1F3"},
    "shortcode": "flag-vn",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS VN",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-vu",
    "unicode": {"apple":"1F1FB-1F1FA", "google":"1F1FB-1F1FA", "twitter":"1F1FB-1F1FA"},
    "shortcode": "flag-vu",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS VU",
    "category": "flag"
  },
  {
    "name": "flag-wf",
    "unicode": {"apple":"1F1FC-1F1EB", "google":"1F1FC-1F1EB", "twitter":"1F1FC-1F1EB"},
    "shortcode": "flag-wf",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS WF",
    "category": "flag"
  },
  {
    "name": "flag-ws",
    "unicode": {"apple":"1F1FC-1F1F8", "google":"1F1FC-1F1F8", "twitter":"1F1FC-1F1F8"},
    "shortcode": "flag-ws",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS WS",
    "category": "flag"
  },
  {
    "name": "flag-xk",
    "unicode": {"apple":"1F1FD-1F1F0", "google":"1F1FD-1F1F0", "twitter":"1F1FD-1F1F0"},
    "shortcode": "flag-xk",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS XK",
    "category": "flag"
  },
  {
    "name": "flag-ye",
    "unicode": {"apple":"1F1FE-1F1EA", "google":"1F1FE-1F1EA", "twitter":"1F1FE-1F1EA"},
    "shortcode": "flag-ye",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS YE",
    "category": "flag"
  },
  {
    "name": "flag-yt",
    "unicode": {"apple":"1F1FE-1F1F9", "google":"1F1FE-1F1F9", "twitter":"1F1FE-1F1F9"},
    "shortcode": "flag-yt",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS YT",
    "category": "flag"
  },
  {
    "name": "flag-za",
    "unicode": {"apple":"1F1FF-1F1E6", "google":"1F1FF-1F1E6", "twitter":"1F1FF-1F1E6"},
    "shortcode": "flag-za",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS ZA",
    "category": "folderol",
    "category": "flag"
  },
  {
    "name": "flag-zm",
    "unicode": {"apple":"1F1FF-1F1F2", "google":"1F1FF-1F1F2", "twitter":"1F1FF-1F1F2"},
    "shortcode": "flag-zm",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS ZM",
    "category": "flag"
  },
  {
    "name": "flag-zw",
    "unicode": {"apple":"1F1FF-1F1FC", "google":"1F1FF-1F1FC", "twitter":"1F1FF-1F1FC"},
    "shortcode": "flag-zw",
    "description": "REGIONAL INDICATOR SYMBOL LETTERS ZW",
    "category": "flag"
  }
]
});

$(function() {
    $.fn.emojiPicker.emojis = [
        {
            "name": "grinning",
            "unicode": { "apple": "1F600", "google": "1F600", "twitter": "1F600" },
            "shortcode": "grinning",
            "category": "people"
        },
        {
            "name": "grin",
            "unicode": { "apple": "1F601", "google": "1F601", "twitter": "1F601" },
            "shortcode": "grin",
            "category": "people"
        },
        {
            "name": "joy",
            "unicode": { "apple": "1F602", "google": "1F602", "twitter": "1F602" },
            "shortcode": "joy",
            "category": "people"
        },
        {
            "name": "rolling_on_the_floor_laughing",
            "unicode": { "apple": "1F923", "google": "1F923", "twitter": "1F923" },
            "shortcode": "rolling_on_the_floor_laughing",
            "category": "people"
        },
        {
            "name": "smiley",
            "unicode": { "apple": "1F603", "google": "1F603", "twitter": "1F603" },
            "shortcode": "smiley",
            "category": "people"
        },
        {
            "name": "smile",
            "unicode": { "apple": "1F604", "google": "1F604", "twitter": "1F604" },
            "shortcode": "smile",
            "category": "people"
        },
        {
            "name": "sweat_smile",
            "unicode": { "apple": "1F605", "google": "1F605", "twitter": "1F605" },
            "shortcode": "sweat_smile",
            "category": "people"
        },
        {
            "name": "laughing",
            "unicode": { "apple": "1F606", "google": "1F606", "twitter": "1F606" },
            "shortcode": "laughing",
            "category": "people"
        },
        {
            "name": "wink",
            "unicode": { "apple": "1F609", "google": "1F609", "twitter": "1F609" },
            "shortcode": "wink",
            "category": "people"
        },
        {
            "name": "blush",
            "unicode": { "apple": "1F60A", "google": "1F60A", "twitter": "1F60A" },
            "shortcode": "blush",
            "category": "people"
        },
        {
            "name": "yum",
            "unicode": { "apple": "1F60B", "google": "1F60B", "twitter": "1F60B" },
            "shortcode": "yum",
            "category": "people"
        },
        {
            "name": "sunglasses",
            "unicode": { "apple": "1F60E", "google": "1F60E", "twitter": "1F60E" },
            "shortcode": "sunglasses",
            "category": "people"
        },
        {
            "name": "heart_eyes",
            "unicode": { "apple": "1F60D", "google": "1F60D", "twitter": "1F60D" },
            "shortcode": "heart_eyes",
            "category": "people"
        },
        {
            "name": "relaxed",
            "unicode": { "apple": "263A-FE0F", "google": "263A-FE0F", "twitter": "263A-FE0F" },
            "shortcode": "relaxed",
            "category": "people"
        },
        {
            "name": "slightly_smiling_face",
            "unicode": { "apple": "1F642", "google": "1F642", "twitter": "1F642" },
            "shortcode": "slightly_smiling_face",
            "category": "people"
        },
        {
            "name": "hugging_face",
            "unicode": { "apple": "1F917", "google": "1F917", "twitter": "1F917" },
            "shortcode": "hugging_face",
            "category": "people"
        },
        {
            "name": "star-struck",
            "unicode": { "apple": "1F929", "google": "1F929", "twitter": "1F929" },
            "shortcode": "star-struck",
            "category": "people"
        },
        {
            "name": "thinking_face",
            "unicode": { "apple": "1F914", "google": "1F914", "twitter": "1F914" },
            "shortcode": "thinking_face",
            "category": "people"
        },
        {
            "name": "face_with_raised_eyebrow",
            "unicode": { "apple": "1F928", "google": "1F928", "twitter": "1F928" },
            "shortcode": "face_with_raised_eyebrow",
            "category": "people"
        },
        {
            "name": "neutral_face",
            "unicode": { "apple": "1F610", "google": "1F610", "twitter": "1F610" },
            "shortcode": "neutral_face",
            "category": "people"
        },
        {
            "name": "expressionless",
            "unicode": { "apple": "1F611", "google": "1F611", "twitter": "1F611" },
            "shortcode": "expressionless",
            "category": "people"
        },
        {
            "name": "no_mouth",
            "unicode": { "apple": "1F636", "google": "1F636", "twitter": "1F636" },
            "shortcode": "no_mouth",
            "category": "people"
        },
        {
            "name": "face_with_rolling_eyes",
            "unicode": { "apple": "1F644", "google": "1F644", "twitter": "1F644" },
            "shortcode": "face_with_rolling_eyes",
            "category": "people"
        },
        {
            "name": "smirk",
            "unicode": { "apple": "1F60F", "google": "1F60F", "twitter": "1F60F" },
            "shortcode": "smirk",
            "category": "people"
        },
        {
            "name": "open_mouth",
            "unicode": { "apple": "1F62E", "google": "1F62E", "twitter": "1F62E" },
            "shortcode": "open_mouth",
            "category": "people"
        },
        {
            "name": "zipper_mouth_face",
            "unicode": { "apple": "1F910", "google": "1F910", "twitter": "1F910" },
            "shortcode": "zipper_mouth_face",
            "category": "people"
        },
        {
            "name": "hushed",
            "unicode": { "apple": "1F62F", "google": "1F62F", "twitter": "1F62F" },
            "shortcode": "hushed",
            "category": "people"
        },
        {
            "name": "sleeping",
            "unicode": { "apple": "1F634", "google": "1F634", "twitter": "1F634" },
            "shortcode": "sleeping",
            "category": "people"
        },
        {
            "name": "relieved",
            "unicode": { "apple": "1F60C", "google": "1F60C", "twitter": "1F60C" },
            "shortcode": "relieved",
            "category": "people"
        },
        {
            "name": "drooling_face",
            "unicode": { "apple": "1F924", "google": "1F924", "twitter": "1F924" },
            "shortcode": "drooling_face",
            "category": "people"
        },
        {
            "name": "unamused",
            "unicode": { "apple": "1F612", "google": "1F612", "twitter": "1F612" },
            "shortcode": "unamused",
            "category": "people"
        },
        {
            "name": "sweat",
            "unicode": { "apple": "1F613", "google": "1F613", "twitter": "1F613" },
            "shortcode": "sweat",
            "category": "people"
        },
        {
            "name": "pensive",
            "unicode": { "apple": "1F614", "google": "1F614", "twitter": "1F614" },
            "shortcode": "pensive",
            "category": "people"
        },
        {
            "name": "upside_down_face",
            "unicode": { "apple": "1F643", "google": "1F643", "twitter": "1F643" },
            "shortcode": "upside_down_face",
            "category": "people"
        },
        {
            "name": "money_mouth_face",
            "unicode": { "apple": "1F911", "google": "1F911", "twitter": "1F911" },
            "shortcode": "money_mouth_face",
            "category": "people"
        },
        {
            "name": "astonished",
            "unicode": { "apple": "1F632", "google": "1F632", "twitter": "1F632" },
            "shortcode": "astonished",
            "category": "people"
        },
        {
            "name": "white_frowning_face",
            "unicode": { "apple": "2639-FE0F", "google": "2639-FE0F", "twitter": "2639-FE0F" },
            "shortcode": "white_frowning_face",
            "category": "people"
        },
        {
            "name": "slightly_frowning_face",
            "unicode": { "apple": "1F641", "google": "1F641", "twitter": "1F641" },
            "shortcode": "slightly_frowning_face",
            "category": "people"
        },
        {
            "name": "sob",
            "unicode": { "apple": "1F62D", "google": "1F62D", "twitter": "1F62D" },
            "shortcode": "sob",
            "category": "people"
        },
        {
            "name": "exploding_head",
            "unicode": { "apple": "1F92F", "google": "1F92F", "twitter": "1F92F" },
            "shortcode": "exploding_head",
            "category": "people"
        },
        {
            "name": "cold_sweat",
            "unicode": { "apple": "1F630", "google": "1F630", "twitter": "1F630" },
            "shortcode": "cold_sweat",
            "category": "people"
        },
        {
            "name": "scream",
            "unicode": { "apple": "1F631", "google": "1F631", "twitter": "1F631" },
            "shortcode": "scream",
            "category": "people"
        },
        {
            "name": "flushed",
            "unicode": { "apple": "1F633", "google": "1F633", "twitter": "1F633" },
            "shortcode": "flushed",
            "category": "people"
        },
        {
            "name": "zany_face",
            "unicode": { "apple": "1F92A", "google": "1F92A", "twitter": "1F92A" },
            "shortcode": "zany_face",
            "category": "people"
        },
        {
            "name": "dizzy_face",
            "unicode": { "apple": "1F635", "google": "1F635", "twitter": "1F635" },
            "shortcode": "dizzy_face",
            "category": "people"
        },
        {
            "name": "face_with_symbols_on_mouth",
            "unicode": { "apple": "1F92C", "google": "1F92C", "twitter": "1F92C" },
            "shortcode": "face_with_symbols_on_mouth",
            "category": "people"
        },
        {
            "name": "mask",
            "unicode": { "apple": "1F637", "google": "1F637", "twitter": "1F637" },
            "shortcode": "mask",
            "category": "people"
        },
        {
            "name": "face_with_thermometer",
            "unicode": { "apple": "1F912", "google": "1F912", "twitter": "1F912" },
            "shortcode": "face_with_thermometer",
            "category": "people"
        },
        {
            "name": "face_with_head_bandage",
            "unicode": { "apple": "1F915", "google": "1F915", "twitter": "1F915" },
            "shortcode": "face_with_head_bandage",
            "category": "people"
        },
        {
            "name": "nauseated_face",
            "unicode": { "apple": "1F922", "google": "1F922", "twitter": "1F922" },
            "shortcode": "nauseated_face",
            "category": "people"
        },
        {
            "name": "face_vomiting",
            "unicode": { "apple": "1F92E", "google": "1F92E", "twitter": "1F92E" },
            "shortcode": "face_vomiting",
            "category": "people"
        },
        {
            "name": "sneezing_face",
            "unicode": { "apple": "1F927", "google": "1F927", "twitter": "1F927" },
            "shortcode": "sneezing_face",
            "category": "people"
        },
        {
            "name": "innocent",
            "unicode": { "apple": "1F607", "google": "1F607", "twitter": "1F607" },
            "shortcode": "innocent",
            "category": "people"
        },
        {
            "name": "face_with_cowboy_hat",
            "unicode": { "apple": "1F920", "google": "1F920", "twitter": "1F920" },
            "shortcode": "face_with_cowboy_hat",
            "category": "people"
        },
        {
            "name": "clown_face",
            "unicode": { "apple": "1F921", "google": "1F921", "twitter": "1F921" },
            "shortcode": "clown_face",
            "category": "people"
        },
        {
            "name": "lying_face",
            "unicode": { "apple": "1F925", "google": "1F925", "twitter": "1F925" },
            "shortcode": "lying_face",
            "category": "people"
        },
        {
            "name": "shushing_face",
            "unicode": { "apple": "1F92B", "google": "1F92B", "twitter": "1F92B" },
            "shortcode": "shushing_face",
            "category": "people"
        },
        {
            "name": "face_with_hand_over_mouth",
            "unicode": { "apple": "1F92D", "google": "1F92D", "twitter": "1F92D" },
            "shortcode": "face_with_hand_over_mouth",
            "category": "people"
        },
        {
            "name": "face_with_monocle",
            "unicode": { "apple": "1F9D0", "google": "1F9D0", "twitter": "1F9D0" },
            "shortcode": "face_with_monocle",
            "category": "people"
        },
        {
            "name": "nerd_face",
            "unicode": { "apple": "1F913", "google": "1F913", "twitter": "1F913" },
            "shortcode": "nerd_face",
            "category": "people"
        },
        {
            "name": "smiling_imp",
            "unicode": { "apple": "1F608", "google": "1F608", "twitter": "1F608" },
            "shortcode": "smiling_imp",
            "category": "people"
        },
        {
            "name": "imp",
            "unicode": { "apple": "1F47F", "google": "1F47F", "twitter": "1F47F" },
            "shortcode": "imp",
            "category": "people"
        },
        {
            "name": "japanese_ogre",
            "unicode": { "apple": "1F479", "google": "1F479", "twitter": "1F479" },
            "shortcode": "japanese_ogre",
            "category": "people"
        },
        {
            "name": "japanese_goblin",
            "unicode": { "apple": "1F47A", "google": "1F47A", "twitter": "1F47A" },
            "shortcode": "japanese_goblin",
            "category": "people"
        },
        {
            "name": "skull",
            "unicode": { "apple": "1F480", "google": "1F480", "twitter": "1F480" },
            "shortcode": "skull",
            "category": "people"
        },
        {
            "name": "skull_and_crossbones",
            "unicode": { "apple": "2620-FE0F", "google": "2620-FE0F", "twitter": "2620-FE0F" },
            "shortcode": "skull_and_crossbones",
            "category": "people"
        },
        {
            "name": "ghost",
            "unicode": { "apple": "1F47B", "google": "1F47B", "twitter": "1F47B" },
            "shortcode": "ghost",
            "category": "people"
        },
        {
            "name": "alien",
            "unicode": { "apple": "1F47D", "google": "1F47D", "twitter": "1F47D" },
            "shortcode": "alien",
            "category": "people"
        },
        {
            "name": "space_invader",
            "unicode": { "apple": "1F47E", "google": "1F47E", "twitter": "1F47E" },
            "shortcode": "space_invader",
            "category": "people"
        },
        {
            "name": "robot_face",
            "unicode": { "apple": "1F916", "google": "1F916", "twitter": "1F916" },
            "shortcode": "robot_face",
            "category": "people"
        },
        {
            "name": "hankey",
            "unicode": { "apple": "1F4A9", "google": "1F4A9", "twitter": "1F4A9" },
            "shortcode": "hankey",
            "category": "people"
        },
        {
            "name": "smiley_cat",
            "unicode": { "apple": "1F63A", "google": "1F63A", "twitter": "1F63A" },
            "shortcode": "smiley_cat",
            "category": "people"
        },
        {
            "name": "smile_cat",
            "unicode": { "apple": "1F638", "google": "1F638", "twitter": "1F638" },
            "shortcode": "smile_cat",
            "category": "people"
        },
        {
            "name": "joy_cat",
            "unicode": { "apple": "1F639", "google": "1F639", "twitter": "1F639" },
            "shortcode": "joy_cat",
            "category": "people"
        },
        {
            "name": "heart_eyes_cat",
            "unicode": { "apple": "1F63B", "google": "1F63B", "twitter": "1F63B" },
            "shortcode": "heart_eyes_cat",
            "category": "people"
        },
        {
            "name": "smirk_cat",
            "unicode": { "apple": "1F63C", "google": "1F63C", "twitter": "1F63C" },
            "shortcode": "smirk_cat",
            "category": "people"
        },
        {
            "name": "kissing_cat",
            "unicode": { "apple": "1F63D", "google": "1F63D", "twitter": "1F63D" },
            "shortcode": "kissing_cat",
            "category": "people"
        },
        {
            "name": "scream_cat",
            "unicode": { "apple": "1F640", "google": "1F640", "twitter": "1F640" },
            "shortcode": "scream_cat",
            "category": "people"
        },
        {
            "name": "crying_cat_face",
            "unicode": { "apple": "1F63F", "google": "1F63F", "twitter": "1F63F" },
            "shortcode": "crying_cat_face",
            "category": "people"
        },
        {
            "name": "pouting_cat",
            "unicode": { "apple": "1F63E", "google": "1F63E", "twitter": "1F63E" },
            "shortcode": "pouting_cat",
            "category": "people"
        },
        {
            "name": "see_no_evil",
            "unicode": { "apple": "1F648", "google": "1F648", "twitter": "1F648" },
            "shortcode": "see_no_evil",
            "category": "people"
        },
        {
            "name": "hear_no_evil",
            "unicode": { "apple": "1F649", "google": "1F649", "twitter": "1F649" },
            "shortcode": "hear_no_evil",
            "category": "people"
        },
        {
            "name": "speak_no_evil",
            "unicode": { "apple": "1F64A", "google": "1F64A", "twitter": "1F64A" },
            "shortcode": "speak_no_evil",
            "category": "people"
        },
        {
            "name": "baby",
            "unicode": { "apple": "1F476", "google": "1F476", "twitter": "1F476" },
            "shortcode": "baby",
            "category": "people"
        },
        {
            "name": "child",
            "unicode": { "apple": "1F9D2", "google": "1F9D2", "twitter": "1F9D2" },
            "shortcode": "child",
            "category": "people"
        },
        {
            "name": "boy",
            "unicode": { "apple": "1F466", "google": "1F466", "twitter": "1F466" },
            "shortcode": "boy",
            "category": "people"
        },
        {
            "name": "girl",
            "unicode": { "apple": "1F467", "google": "1F467", "twitter": "1F467" },
            "shortcode": "girl",
            "category": "people"
        },
        {
            "name": "adult",
            "unicode": { "apple": "1F9D1", "google": "1F9D1", "twitter": "1F9D1" },
            "shortcode": "adult",
            "category": "people"
        },
        {
            "name": "man",
            "unicode": { "apple": "1F468", "google": "1F468", "twitter": "1F468" },
            "shortcode": "man",
            "category": "people"
        },
        {
            "name": "woman",
            "unicode": { "apple": "1F469", "google": "1F469", "twitter": "1F469" },
            "shortcode": "woman",
            "category": "people"
        },
        {
            "name": "older_adult",
            "unicode": { "apple": "1F9D3", "google": "1F9D3", "twitter": "1F9D3" },
            "shortcode": "older_adult",
            "category": "people"
        },
        {
            "name": "older_man",
            "unicode": { "apple": "1F474", "google": "1F474", "twitter": "1F474" },
            "shortcode": "older_man",
            "category": "people"
        },
        {
            "name": "older_woman",
            "unicode": { "apple": "1F475", "google": "1F475", "twitter": "1F475" },
            "shortcode": "older_woman",
            "category": "people"
        },
        {
            "name": "male-doctor",
            "unicode": {
                "apple": "1F468-200D-2695-FE0F",
                "google": "1F468-200D-2695-FE0F",
                "twitter": "1F468-200D-2695-FE0F"
            },
            "shortcode": "male-doctor",
            "category": "people"
        },
        {
            "name": "female-doctor",
            "unicode": {
                "apple": "1F469-200D-2695-FE0F",
                "google": "1F469-200D-2695-FE0F",
                "twitter": "1F469-200D-2695-FE0F"
            },
            "shortcode": "female-doctor",
            "category": "people"
        },
        {
            "name": "male-student",
            "unicode": { "apple": "1F468-200D-1F393", "google": "1F468-200D-1F393", "twitter": "1F468-200D-1F393" },
            "shortcode": "male-student",
            "category": "people"
        },
        {
            "name": "female-student",
            "unicode": { "apple": "1F469-200D-1F393", "google": "1F469-200D-1F393", "twitter": "1F469-200D-1F393" },
            "shortcode": "female-student",
            "category": "people"
        },
        {
            "name": "male-teacher",
            "unicode": { "apple": "1F468-200D-1F3EB", "google": "1F468-200D-1F3EB", "twitter": "1F468-200D-1F3EB" },
            "shortcode": "male-teacher",
            "category": "people"
        },
        {
            "name": "female-teacher",
            "unicode": { "apple": "1F469-200D-1F3EB", "google": "1F469-200D-1F3EB", "twitter": "1F469-200D-1F3EB" },
            "shortcode": "female-teacher",
            "category": "people"
        },
        {
            "name": "male-judge",
            "unicode": {
                "apple": "1F468-200D-2696-FE0F",
                "google": "1F468-200D-2696-FE0F",
                "twitter": "1F468-200D-2696-FE0F"
            },
            "shortcode": "male-judge",
            "category": "people"
        },
        {
            "name": "female-judge",
            "unicode": {
                "apple": "1F469-200D-2696-FE0F",
                "google": "1F469-200D-2696-FE0F",
                "twitter": "1F469-200D-2696-FE0F"
            },
            "shortcode": "female-judge",
            "category": "people"
        },
        {
            "name": "male-farmer",
            "unicode": { "apple": "1F468-200D-1F33E", "google": "1F468-200D-1F33E", "twitter": "1F468-200D-1F33E" },
            "shortcode": "male-farmer",
            "category": "people"
        },
        {
            "name": "female-farmer",
            "unicode": { "apple": "1F469-200D-1F33E", "google": "1F469-200D-1F33E", "twitter": "1F469-200D-1F33E" },
            "shortcode": "female-farmer",
            "category": "people"
        },
        {
            "name": "male-cook",
            "unicode": { "apple": "1F468-200D-1F373", "google": "1F468-200D-1F373", "twitter": "1F468-200D-1F373" },
            "shortcode": "male-cook",
            "category": "people"
        },
        {
            "name": "female-cook",
            "unicode": { "apple": "1F469-200D-1F373", "google": "1F469-200D-1F373", "twitter": "1F469-200D-1F373" },
            "shortcode": "female-cook",
            "category": "people"
        },
        {
            "name": "male-mechanic",
            "unicode": { "apple": "1F468-200D-1F527", "google": "1F468-200D-1F527", "twitter": "1F468-200D-1F527" },
            "shortcode": "male-mechanic",
            "category": "people"
        },
        {
            "name": "female-mechanic",
            "unicode": { "apple": "1F469-200D-1F527", "google": "1F469-200D-1F527", "twitter": "1F469-200D-1F527" },
            "shortcode": "female-mechanic",
            "category": "people"
        },
        {
            "name": "male-factory-worker",
            "unicode": { "apple": "1F468-200D-1F3ED", "google": "1F468-200D-1F3ED", "twitter": "1F468-200D-1F3ED" },
            "shortcode": "male-factory-worker",
            "category": "people"
        },
        {
            "name": "female-factory-worker",
            "unicode": { "apple": "1F469-200D-1F3ED", "google": "1F469-200D-1F3ED", "twitter": "1F469-200D-1F3ED" },
            "shortcode": "female-factory-worker",
            "category": "people"
        },
        {
            "name": "male-office-worker",
            "unicode": { "apple": "1F468-200D-1F4BC", "google": "1F468-200D-1F4BC", "twitter": "1F468-200D-1F4BC" },
            "shortcode": "male-office-worker",
            "category": "people"
        },
        {
            "name": "female-office-worker",
            "unicode": { "apple": "1F469-200D-1F4BC", "google": "1F469-200D-1F4BC", "twitter": "1F469-200D-1F4BC" },
            "shortcode": "female-office-worker",
            "category": "people"
        },
        {
            "name": "male-scientist",
            "unicode": { "apple": "1F468-200D-1F52C", "google": "1F468-200D-1F52C", "twitter": "1F468-200D-1F52C" },
            "shortcode": "male-scientist",
            "category": "people"
        },
        {
            "name": "female-scientist",
            "unicode": { "apple": "1F469-200D-1F52C", "google": "1F469-200D-1F52C", "twitter": "1F469-200D-1F52C" },
            "shortcode": "female-scientist",
            "category": "people"
        },
        {
            "name": "male-technologist",
            "unicode": { "apple": "1F468-200D-1F4BB", "google": "1F468-200D-1F4BB", "twitter": "1F468-200D-1F4BB" },
            "shortcode": "male-technologist",
            "category": "people"
        },
        {
            "name": "female-technologist",
            "unicode": { "apple": "1F469-200D-1F4BB", "google": "1F469-200D-1F4BB", "twitter": "1F469-200D-1F4BB" },
            "shortcode": "female-technologist",
            "category": "people"
        },
        {
            "name": "male-singer",
            "unicode": { "apple": "1F468-200D-1F3A4", "google": "1F468-200D-1F3A4", "twitter": "1F468-200D-1F3A4" },
            "shortcode": "male-singer",
            "category": "people"
        },
        {
            "name": "female-singer",
            "unicode": { "apple": "1F469-200D-1F3A4", "google": "1F469-200D-1F3A4", "twitter": "1F469-200D-1F3A4" },
            "shortcode": "female-singer",
            "category": "people"
        },
        {
            "name": "male-artist",
            "unicode": { "apple": "1F468-200D-1F3A8", "google": "1F468-200D-1F3A8", "twitter": "1F468-200D-1F3A8" },
            "shortcode": "male-artist",
            "category": "people"
        },
        {
            "name": "female-artist",
            "unicode": { "apple": "1F469-200D-1F3A8", "google": "1F469-200D-1F3A8", "twitter": "1F469-200D-1F3A8" },
            "shortcode": "female-artist",
            "category": "people"
        },
        {
            "name": "male-pilot",
            "unicode": {
                "apple": "1F468-200D-2708-FE0F",
                "google": "1F468-200D-2708-FE0F",
                "twitter": "1F468-200D-2708-FE0F"
            },
            "shortcode": "male-pilot",
            "category": "people"
        },
        {
            "name": "female-pilot",
            "unicode": {
                "apple": "1F469-200D-2708-FE0F",
                "google": "1F469-200D-2708-FE0F",
                "twitter": "1F469-200D-2708-FE0F"
            },
            "shortcode": "female-pilot",
            "category": "people"
        },
        {
            "name": "male-astronaut",
            "unicode": { "apple": "1F468-200D-1F680", "google": "1F468-200D-1F680", "twitter": "1F468-200D-1F680" },
            "shortcode": "male-astronaut",
            "category": "people"
        },
        {
            "name": "female-astronaut",
            "unicode": { "apple": "1F469-200D-1F680", "google": "1F469-200D-1F680", "twitter": "1F469-200D-1F680" },
            "shortcode": "female-astronaut",
            "category": "people"
        },
        {
            "name": "male-firefighter",
            "unicode": { "apple": "1F468-200D-1F692", "google": "1F468-200D-1F692", "twitter": "1F468-200D-1F692" },
            "shortcode": "male-firefighter",
            "category": "people"
        },
        {
            "name": "female-firefighter",
            "unicode": { "apple": "1F469-200D-1F692", "google": "1F469-200D-1F692", "twitter": "1F469-200D-1F692" },
            "shortcode": "female-firefighter",
            "category": "people"
        },
        {
            "name": "cop",
            "unicode": { "apple": "1F46E", "google": "1F46E", "twitter": "1F46E" },
            "shortcode": "cop",
            "category": "people"
        },
        {
            "name": "male-police-officer",
            "unicode": {
                "apple": "1F46E-200D-2642-FE0F",
                "google": "1F46E-200D-2642-FE0F",
                "twitter": "1F46E-200D-2642-FE0F"
            },
            "shortcode": "male-police-officer",
            "category": "people"
        },
        {
            "name": "female-police-officer",
            "unicode": {
                "apple": "1F46E-200D-2640-FE0F",
                "google": "1F46E-200D-2640-FE0F",
                "twitter": "1F46E-200D-2640-FE0F"
            },
            "shortcode": "female-police-officer",
            "category": "people"
        },
        {
            "name": "sleuth_or_spy",
            "unicode": { "apple": "1F575-FE0F", "google": "1F575-FE0F", "twitter": "1F575-FE0F" },
            "shortcode": "sleuth_or_spy",
            "category": "people"
        },
        {
            "name": "male-detective",
            "unicode": {
                "apple": "1F575-FE0F-200D-2642-FE0F",
                "google": "1F575-FE0F-200D-2642-FE0F",
                "twitter": "1F575-FE0F-200D-2642-FE0F"
            },
            "shortcode": "male-detective",
            "category": "people"
        },
        {
            "name": "female-detective",
            "unicode": {
                "apple": "1F575-FE0F-200D-2640-FE0F",
                "google": "1F575-FE0F-200D-2640-FE0F",
                "twitter": "1F575-FE0F-200D-2640-FE0F"
            },
            "shortcode": "female-detective",
            "category": "people"
        },
        {
            "name": "guardsman",
            "unicode": { "apple": "1F482", "google": "1F482", "twitter": "1F482" },
            "shortcode": "guardsman",
            "category": "people"
        },
        {
            "name": "male-guard",
            "unicode": {
                "apple": "1F482-200D-2642-FE0F",
                "google": "1F482-200D-2642-FE0F",
                "twitter": "1F482-200D-2642-FE0F"
            },
            "shortcode": "male-guard",
            "category": "people"
        },
        {
            "name": "female-guard",
            "unicode": {
                "apple": "1F482-200D-2640-FE0F",
                "google": "1F482-200D-2640-FE0F",
                "twitter": "1F482-200D-2640-FE0F"
            },
            "shortcode": "female-guard",
            "category": "people"
        },
        {
            "name": "construction_worker",
            "unicode": { "apple": "1F477", "google": "1F477", "twitter": "1F477" },
            "shortcode": "construction_worker",
            "category": "people"
        },
        {
            "name": "male-construction-worker",
            "unicode": {
                "apple": "1F477-200D-2642-FE0F",
                "google": "1F477-200D-2642-FE0F",
                "twitter": "1F477-200D-2642-FE0F"
            },
            "shortcode": "male-construction-worker",
            "category": "people"
        },
        {
            "name": "female-construction-worker",
            "unicode": {
                "apple": "1F477-200D-2640-FE0F",
                "google": "1F477-200D-2640-FE0F",
                "twitter": "1F477-200D-2640-FE0F"
            },
            "shortcode": "female-construction-worker",
            "category": "people"
        },
        {
            "name": "prince",
            "unicode": { "apple": "1F934", "google": "1F934", "twitter": "1F934" },
            "shortcode": "prince",
            "category": "people"
        },
        {
            "name": "princess",
            "unicode": { "apple": "1F478", "google": "1F478", "twitter": "1F478" },
            "shortcode": "princess",
            "category": "people"
        },
        {
            "name": "man_with_turban",
            "unicode": { "apple": "1F473", "google": "1F473", "twitter": "1F473" },
            "shortcode": "man_with_turban",
            "category": "people"
        },
        {
            "name": "man-wearing-turban",
            "unicode": {
                "apple": "1F473-200D-2642-FE0F",
                "google": "1F473-200D-2642-FE0F",
                "twitter": "1F473-200D-2642-FE0F"
            },
            "shortcode": "man-wearing-turban",
            "category": "people"
        },
        {
            "name": "woman-wearing-turban",
            "unicode": {
                "apple": "1F473-200D-2640-FE0F",
                "google": "1F473-200D-2640-FE0F",
                "twitter": "1F473-200D-2640-FE0F"
            },
            "shortcode": "woman-wearing-turban",
            "category": "people"
        },
        {
            "name": "man_with_gua_pi_mao",
            "unicode": { "apple": "1F472", "google": "1F472", "twitter": "1F472" },
            "shortcode": "man_with_gua_pi_mao",
            "category": "people"
        },
        {
            "name": "person_with_headscarf",
            "unicode": { "apple": "1F9D5", "google": "1F9D5", "twitter": "1F9D5" },
            "shortcode": "person_with_headscarf",
            "category": "people"
        },
        {
            "name": "bearded_person",
            "unicode": { "apple": "1F9D4", "google": "1F9D4", "twitter": "1F9D4" },
            "shortcode": "bearded_person",
            "category": "people"
        },
        {
            "name": "person_with_blond_hair",
            "unicode": { "apple": "1F471", "google": "1F471", "twitter": "1F471" },
            "shortcode": "person_with_blond_hair",
            "category": "people"
        },
        {
            "name": "blond-haired-man",
            "unicode": {
                "apple": "1F471-200D-2642-FE0F",
                "google": "1F471-200D-2642-FE0F",
                "twitter": "1F471-200D-2642-FE0F"
            },
            "shortcode": "blond-haired-man",
            "category": "people"
        },
        {
            "name": "blond-haired-woman",
            "unicode": {
                "apple": "1F471-200D-2640-FE0F",
                "google": "1F471-200D-2640-FE0F",
                "twitter": "1F471-200D-2640-FE0F"
            },
            "shortcode": "blond-haired-woman",
            "category": "people"
        },
        {
            "name": "man_in_tuxedo",
            "unicode": { "apple": "1F935", "google": "1F935", "twitter": "1F935" },
            "shortcode": "man_in_tuxedo",
            "category": "people"
        },
        {
            "name": "bride_with_veil",
            "unicode": { "apple": "1F470", "google": "1F470", "twitter": "1F470" },
            "shortcode": "bride_with_veil",
            "category": "people"
        },
        {
            "name": "pregnant_woman",
            "unicode": { "apple": "1F930", "google": "1F930", "twitter": "1F930" },
            "shortcode": "pregnant_woman",
            "category": "people"
        },
        {
            "name": "breast-feeding",
            "unicode": { "apple": "1F931", "google": "1F931", "twitter": "1F931" },
            "shortcode": "breast-feeding",
            "category": "people"
        },
        {
            "name": "angel",
            "unicode": { "apple": "1F47C", "google": "1F47C", "twitter": "1F47C" },
            "shortcode": "angel",
            "category": "people"
        },
        {
            "name": "santa",
            "unicode": { "apple": "1F385", "google": "1F385", "twitter": "1F385" },
            "shortcode": "santa",
            "category": "people"
        },
        {
            "name": "mrs_claus",
            "unicode": { "apple": "1F936", "google": "1F936", "twitter": "1F936" },
            "shortcode": "mrs_claus",
            "category": "people"
        },
        {
            "name": "mage",
            "unicode": { "apple": "1F9D9", "google": "1F9D9", "twitter": "1F9D9" },
            "shortcode": "mage",
            "category": "people"
        },
        {
            "name": "female_mage",
            "unicode": {
                "apple": "1F9D9-200D-2640-FE0F",
                "google": "1F9D9-200D-2640-FE0F",
                "twitter": "1F9D9-200D-2640-FE0F"
            },
            "shortcode": "female_mage",
            "category": "people"
        },
        {
            "name": "male_mage",
            "unicode": {
                "apple": "1F9D9-200D-2642-FE0F",
                "google": "1F9D9-200D-2642-FE0F",
                "twitter": "1F9D9-200D-2642-FE0F"
            },
            "shortcode": "male_mage",
            "category": "people"
        },
        {
            "name": "fairy",
            "unicode": { "apple": "1F9DA", "google": "1F9DA", "twitter": "1F9DA" },
            "shortcode": "fairy",
            "category": "people"
        },
        {
            "name": "female_fairy",
            "unicode": {
                "apple": "1F9DA-200D-2640-FE0F",
                "google": "1F9DA-200D-2640-FE0F",
                "twitter": "1F9DA-200D-2640-FE0F"
            },
            "shortcode": "female_fairy",
            "category": "people"
        },
        {
            "name": "male_fairy",
            "unicode": {
                "apple": "1F9DA-200D-2642-FE0F",
                "google": "1F9DA-200D-2642-FE0F",
                "twitter": "1F9DA-200D-2642-FE0F"
            },
            "shortcode": "male_fairy",
            "category": "people"
        },
        {
            "name": "vampire",
            "unicode": { "apple": "1F9DB", "google": "1F9DB", "twitter": "1F9DB" },
            "shortcode": "vampire",
            "category": "people"
        },
        {
            "name": "female_vampire",
            "unicode": {
                "apple": "1F9DB-200D-2640-FE0F",
                "google": "1F9DB-200D-2640-FE0F",
                "twitter": "1F9DB-200D-2640-FE0F"
            },
            "shortcode": "female_vampire",
            "category": "people"
        },
        {
            "name": "male_vampire",
            "unicode": {
                "apple": "1F9DB-200D-2642-FE0F",
                "google": "1F9DB-200D-2642-FE0F",
                "twitter": "1F9DB-200D-2642-FE0F"
            },
            "shortcode": "male_vampire",
            "category": "people"
        },
        {
            "name": "merperson",
            "unicode": { "apple": "1F9DC", "google": "1F9DC", "twitter": "1F9DC" },
            "shortcode": "merperson",
            "category": "people"
        },
        {
            "name": "mermaid",
            "unicode": {
                "apple": "1F9DC-200D-2640-FE0F",
                "google": "1F9DC-200D-2640-FE0F",
                "twitter": "1F9DC-200D-2640-FE0F"
            },
            "shortcode": "mermaid",
            "category": "people"
        },
        {
            "name": "merman",
            "unicode": {
                "apple": "1F9DC-200D-2642-FE0F",
                "google": "1F9DC-200D-2642-FE0F",
                "twitter": "1F9DC-200D-2642-FE0F"
            },
            "shortcode": "merman",
            "category": "people"
        },
        {
            "name": "elf",
            "unicode": { "apple": "1F9DD", "google": "1F9DD", "twitter": "1F9DD" },
            "shortcode": "elf",
            "category": "people"
        },
        {
            "name": "female_elf",
            "unicode": {
                "apple": "1F9DD-200D-2640-FE0F",
                "google": "1F9DD-200D-2640-FE0F",
                "twitter": "1F9DD-200D-2640-FE0F"
            },
            "shortcode": "female_elf",
            "category": "people"
        },
        {
            "name": "male_elf",
            "unicode": {
                "apple": "1F9DD-200D-2642-FE0F",
                "google": "1F9DD-200D-2642-FE0F",
                "twitter": "1F9DD-200D-2642-FE0F"
            },
            "shortcode": "male_elf",
            "category": "people"
        },
        {
            "name": "genie",
            "unicode": { "apple": "1F9DE", "google": "1F9DE", "twitter": "1F9DE" },
            "shortcode": "genie",
            "category": "people"
        },
        {
            "name": "female_genie",
            "unicode": {
                "apple": "1F9DE-200D-2640-FE0F",
                "google": "1F9DE-200D-2640-FE0F",
                "twitter": "1F9DE-200D-2640-FE0F"
            },
            "shortcode": "female_genie",
            "category": "people"
        },
        {
            "name": "male_genie",
            "unicode": {
                "apple": "1F9DE-200D-2642-FE0F",
                "google": "1F9DE-200D-2642-FE0F",
                "twitter": "1F9DE-200D-2642-FE0F"
            },
            "shortcode": "male_genie",
            "category": "people"
        },
        {
            "name": "zombie",
            "unicode": { "apple": "1F9DF", "google": "1F9DF", "twitter": "1F9DF" },
            "shortcode": "zombie",
            "category": "people"
        },
        {
            "name": "female_zombie",
            "unicode": {
                "apple": "1F9DF-200D-2640-FE0F",
                "google": "1F9DF-200D-2640-FE0F",
                "twitter": "1F9DF-200D-2640-FE0F"
            },
            "shortcode": "female_zombie",
            "category": "people"
        },
        {
            "name": "male_zombie",
            "unicode": {
                "apple": "1F9DF-200D-2642-FE0F",
                "google": "1F9DF-200D-2642-FE0F",
                "twitter": "1F9DF-200D-2642-FE0F"
            },
            "shortcode": "male_zombie",
            "category": "people"
        },
        {
            "name": "person_frowning",
            "unicode": { "apple": "1F64D", "google": "1F64D", "twitter": "1F64D" },
            "shortcode": "person_frowning",
            "category": "people"
        },
        {
            "name": "man-frowning",
            "unicode": {
                "apple": "1F64D-200D-2642-FE0F",
                "google": "1F64D-200D-2642-FE0F",
                "twitter": "1F64D-200D-2642-FE0F"
            },
            "shortcode": "man-frowning",
            "category": "people"
        },
        {
            "name": "woman-frowning",
            "unicode": {
                "apple": "1F64D-200D-2640-FE0F",
                "google": "1F64D-200D-2640-FE0F",
                "twitter": "1F64D-200D-2640-FE0F"
            },
            "shortcode": "woman-frowning",
            "category": "people"
        },
        {
            "name": "person_with_pouting_face",
            "unicode": { "apple": "1F64E", "google": "1F64E", "twitter": "1F64E" },
            "shortcode": "person_with_pouting_face",
            "category": "people"
        },
        {
            "name": "man-pouting",
            "unicode": {
                "apple": "1F64E-200D-2642-FE0F",
                "google": "1F64E-200D-2642-FE0F",
                "twitter": "1F64E-200D-2642-FE0F"
            },
            "shortcode": "man-pouting",
            "category": "people"
        },
        {
            "name": "woman-pouting",
            "unicode": {
                "apple": "1F64E-200D-2640-FE0F",
                "google": "1F64E-200D-2640-FE0F",
                "twitter": "1F64E-200D-2640-FE0F"
            },
            "shortcode": "woman-pouting",
            "category": "people"
        },
        {
            "name": "no_good",
            "unicode": { "apple": "1F645", "google": "1F645", "twitter": "1F645" },
            "shortcode": "no_good",
            "category": "people"
        },
        {
            "name": "man-gesturing-no",
            "unicode": {
                "apple": "1F645-200D-2642-FE0F",
                "google": "1F645-200D-2642-FE0F",
                "twitter": "1F645-200D-2642-FE0F"
            },
            "shortcode": "man-gesturing-no",
            "category": "people"
        },
        {
            "name": "woman-gesturing-no",
            "unicode": {
                "apple": "1F645-200D-2640-FE0F",
                "google": "1F645-200D-2640-FE0F",
                "twitter": "1F645-200D-2640-FE0F"
            },
            "shortcode": "woman-gesturing-no",
            "category": "people"
        },
        {
            "name": "ok_woman",
            "unicode": { "apple": "1F646", "google": "1F646", "twitter": "1F646" },
            "shortcode": "ok_woman",
            "category": "people"
        },
        {
            "name": "man-gesturing-ok",
            "unicode": {
                "apple": "1F646-200D-2642-FE0F",
                "google": "1F646-200D-2642-FE0F",
                "twitter": "1F646-200D-2642-FE0F"
            },
            "shortcode": "man-gesturing-ok",
            "category": "people"
        },
        {
            "name": "woman-gesturing-ok",
            "unicode": {
                "apple": "1F646-200D-2640-FE0F",
                "google": "1F646-200D-2640-FE0F",
                "twitter": "1F646-200D-2640-FE0F"
            },
            "shortcode": "woman-gesturing-ok",
            "category": "people"
        },
        {
            "name": "information_desk_person",
            "unicode": { "apple": "1F481", "google": "1F481", "twitter": "1F481" },
            "shortcode": "information_desk_person",
            "category": "people"
        },
        {
            "name": "man-tipping-hand",
            "unicode": {
                "apple": "1F481-200D-2642-FE0F",
                "google": "1F481-200D-2642-FE0F",
                "twitter": "1F481-200D-2642-FE0F"
            },
            "shortcode": "man-tipping-hand",
            "category": "people"
        },
        {
            "name": "woman-tipping-hand",
            "unicode": {
                "apple": "1F481-200D-2640-FE0F",
                "google": "1F481-200D-2640-FE0F",
                "twitter": "1F481-200D-2640-FE0F"
            },
            "shortcode": "woman-tipping-hand",
            "category": "people"
        },
        {
            "name": "raising_hand",
            "unicode": { "apple": "1F64B", "google": "1F64B", "twitter": "1F64B" },
            "shortcode": "raising_hand",
            "category": "people"
        },
        {
            "name": "man-raising-hand",
            "unicode": {
                "apple": "1F64B-200D-2642-FE0F",
                "google": "1F64B-200D-2642-FE0F",
                "twitter": "1F64B-200D-2642-FE0F"
            },
            "shortcode": "man-raising-hand",
            "category": "people"
        },
        {
            "name": "woman-raising-hand",
            "unicode": {
                "apple": "1F64B-200D-2640-FE0F",
                "google": "1F64B-200D-2640-FE0F",
                "twitter": "1F64B-200D-2640-FE0F"
            },
            "shortcode": "woman-raising-hand",
            "category": "people"
        },
        {
            "name": "bow",
            "unicode": { "apple": "1F647", "google": "1F647", "twitter": "1F647" },
            "shortcode": "bow",
            "category": "people"
        },
        {
            "name": "man-bowing",
            "unicode": {
                "apple": "1F647-200D-2642-FE0F",
                "google": "1F647-200D-2642-FE0F",
                "twitter": "1F647-200D-2642-FE0F"
            },
            "shortcode": "man-bowing",
            "category": "people"
        },
        {
            "name": "woman-bowing",
            "unicode": {
                "apple": "1F647-200D-2640-FE0F",
                "google": "1F647-200D-2640-FE0F",
                "twitter": "1F647-200D-2640-FE0F"
            },
            "shortcode": "woman-bowing",
            "category": "people"
        },
        {
            "name": "face_palm",
            "unicode": { "apple": "1F926", "google": "1F926", "twitter": "1F926" },
            "shortcode": "face_palm",
            "category": "people"
        },
        {
            "name": "man-facepalming",
            "unicode": {
                "apple": "1F926-200D-2642-FE0F",
                "google": "1F926-200D-2642-FE0F",
                "twitter": "1F926-200D-2642-FE0F"
            },
            "shortcode": "man-facepalming",
            "category": "people"
        },
        {
            "name": "woman-facepalming",
            "unicode": {
                "apple": "1F926-200D-2640-FE0F",
                "google": "1F926-200D-2640-FE0F",
                "twitter": "1F926-200D-2640-FE0F"
            },
            "shortcode": "woman-facepalming",
            "category": "people"
        },
        {
            "name": "shrug",
            "unicode": { "apple": "1F937", "google": "1F937", "twitter": "1F937" },
            "shortcode": "shrug",
            "category": "people"
        },
        {
            "name": "man-shrugging",
            "unicode": {
                "apple": "1F937-200D-2642-FE0F",
                "google": "1F937-200D-2642-FE0F",
                "twitter": "1F937-200D-2642-FE0F"
            },
            "shortcode": "man-shrugging",
            "category": "people"
        },
        {
            "name": "woman-shrugging",
            "unicode": {
                "apple": "1F937-200D-2640-FE0F",
                "google": "1F937-200D-2640-FE0F",
                "twitter": "1F937-200D-2640-FE0F"
            },
            "shortcode": "woman-shrugging",
            "category": "people"
        },
        {
            "name": "massage",
            "unicode": { "apple": "1F486", "google": "1F486", "twitter": "1F486" },
            "shortcode": "massage",
            "category": "people"
        },
        {
            "name": "man-getting-massage",
            "unicode": {
                "apple": "1F486-200D-2642-FE0F",
                "google": "1F486-200D-2642-FE0F",
                "twitter": "1F486-200D-2642-FE0F"
            },
            "shortcode": "man-getting-massage",
            "category": "people"
        },
        {
            "name": "woman-getting-massage",
            "unicode": {
                "apple": "1F486-200D-2640-FE0F",
                "google": "1F486-200D-2640-FE0F",
                "twitter": "1F486-200D-2640-FE0F"
            },
            "shortcode": "woman-getting-massage",
            "category": "people"
        },
        {
            "name": "haircut",
            "unicode": { "apple": "1F487", "google": "1F487", "twitter": "1F487" },
            "shortcode": "haircut",
            "category": "people"
        },
        {
            "name": "man-getting-haircut",
            "unicode": {
                "apple": "1F487-200D-2642-FE0F",
                "google": "1F487-200D-2642-FE0F",
                "twitter": "1F487-200D-2642-FE0F"
            },
            "shortcode": "man-getting-haircut",
            "category": "people"
        },
        {
            "name": "woman-getting-haircut",
            "unicode": {
                "apple": "1F487-200D-2640-FE0F",
                "google": "1F487-200D-2640-FE0F",
                "twitter": "1F487-200D-2640-FE0F"
            },
            "shortcode": "woman-getting-haircut",
            "category": "people"
        },
        {
            "name": "walking",
            "unicode": { "apple": "1F6B6", "google": "1F6B6", "twitter": "1F6B6" },
            "shortcode": "walking",
            "category": "people"
        },
        {
            "name": "man-walking",
            "unicode": {
                "apple": "1F6B6-200D-2642-FE0F",
                "google": "1F6B6-200D-2642-FE0F",
                "twitter": "1F6B6-200D-2642-FE0F"
            },
            "shortcode": "man-walking",
            "category": "people"
        },
        {
            "name": "woman-walking",
            "unicode": {
                "apple": "1F6B6-200D-2640-FE0F",
                "google": "1F6B6-200D-2640-FE0F",
                "twitter": "1F6B6-200D-2640-FE0F"
            },
            "shortcode": "woman-walking",
            "category": "people"
        },
        {
            "name": "runner",
            "unicode": { "apple": "1F3C3", "google": "1F3C3", "twitter": "1F3C3" },
            "shortcode": "runner",
            "category": "people"
        },
        {
            "name": "man-running",
            "unicode": {
                "apple": "1F3C3-200D-2642-FE0F",
                "google": "1F3C3-200D-2642-FE0F",
                "twitter": "1F3C3-200D-2642-FE0F"
            },
            "shortcode": "man-running",
            "category": "people"
        },
        {
            "name": "woman-running",
            "unicode": {
                "apple": "1F3C3-200D-2640-FE0F",
                "google": "1F3C3-200D-2640-FE0F",
                "twitter": "1F3C3-200D-2640-FE0F"
            },
            "shortcode": "woman-running",
            "category": "people"
        },
        {
            "name": "dancer",
            "unicode": { "apple": "1F483", "google": "1F483", "twitter": "1F483" },
            "shortcode": "dancer",
            "category": "people"
        },
        {
            "name": "man_dancing",
            "unicode": { "apple": "1F57A", "google": "1F57A", "twitter": "1F57A" },
            "shortcode": "man_dancing",
            "category": "people"
        },
        {
            "name": "dancers",
            "unicode": { "apple": "1F46F", "google": "1F46F", "twitter": "1F46F" },
            "shortcode": "dancers",
            "category": "people"
        },
        {
            "name": "man-with-bunny-ears-partying",
            "unicode": {
                "apple": "1F46F-200D-2642-FE0F",
                "google": "1F46F-200D-2642-FE0F",
                "twitter": "1F46F-200D-2642-FE0F"
            },
            "shortcode": "man-with-bunny-ears-partying",
            "category": "people"
        },
        {
            "name": "woman-with-bunny-ears-partying",
            "unicode": {
                "apple": "1F46F-200D-2640-FE0F",
                "google": "1F46F-200D-2640-FE0F",
                "twitter": "1F46F-200D-2640-FE0F"
            },
            "shortcode": "woman-with-bunny-ears-partying",
            "category": "people"
        },
        {
            "name": "person_in_steamy_room",
            "unicode": { "apple": "1F9D6", "google": "1F9D6", "twitter": "1F9D6" },
            "shortcode": "person_in_steamy_room",
            "category": "people"
        },
        {
            "name": "woman_in_steamy_room",
            "unicode": {
                "apple": "1F9D6-200D-2640-FE0F",
                "google": "1F9D6-200D-2640-FE0F",
                "twitter": "1F9D6-200D-2640-FE0F"
            },
            "shortcode": "woman_in_steamy_room",
            "category": "people"
        },
        {
            "name": "man_in_steamy_room",
            "unicode": {
                "apple": "1F9D6-200D-2642-FE0F",
                "google": "1F9D6-200D-2642-FE0F",
                "twitter": "1F9D6-200D-2642-FE0F"
            },
            "shortcode": "man_in_steamy_room",
            "category": "people"
        },
        {
            "name": "person_climbing",
            "unicode": { "apple": "1F9D7", "google": "1F9D7", "twitter": "1F9D7" },
            "shortcode": "person_climbing",
            "category": "people"
        },
        {
            "name": "woman_climbing",
            "unicode": {
                "apple": "1F9D7-200D-2640-FE0F",
                "google": "1F9D7-200D-2640-FE0F",
                "twitter": "1F9D7-200D-2640-FE0F"
            },
            "shortcode": "woman_climbing",
            "category": "people"
        },
        {
            "name": "man_climbing",
            "unicode": {
                "apple": "1F9D7-200D-2642-FE0F",
                "google": "1F9D7-200D-2642-FE0F",
                "twitter": "1F9D7-200D-2642-FE0F"
            },
            "shortcode": "man_climbing",
            "category": "people"
        },
        {
            "name": "person_in_lotus_position",
            "unicode": { "apple": "1F9D8", "google": "1F9D8", "twitter": "1F9D8" },
            "shortcode": "person_in_lotus_position",
            "category": "people"
        },
        {
            "name": "woman_in_lotus_position",
            "unicode": {
                "apple": "1F9D8-200D-2640-FE0F",
                "google": "1F9D8-200D-2640-FE0F",
                "twitter": "1F9D8-200D-2640-FE0F"
            },
            "shortcode": "woman_in_lotus_position",
            "category": "people"
        },
        {
            "name": "man_in_lotus_position",
            "unicode": {
                "apple": "1F9D8-200D-2642-FE0F",
                "google": "1F9D8-200D-2642-FE0F",
                "twitter": "1F9D8-200D-2642-FE0F"
            },
            "shortcode": "man_in_lotus_position",
            "category": "people"
        },
        {
            "name": "bath",
            "unicode": { "apple": "1F6C0", "google": "1F6C0", "twitter": "1F6C0" },
            "shortcode": "bath",
            "category": "people"
        },
        {
            "name": "sleeping_accommodation",
            "unicode": { "apple": "1F6CC", "google": "1F6CC", "twitter": "1F6CC" },
            "shortcode": "sleeping_accommodation",
            "category": "people"
        },
        {
            "name": "man_in_business_suit_levitating",
            "unicode": { "apple": "1F574-FE0F", "google": "1F574-FE0F", "twitter": "1F574-FE0F" },
            "shortcode": "man_in_business_suit_levitating",
            "category": "people"
        },
        {
            "name": "speaking_head_in_silhouette",
            "unicode": { "apple": "1F5E3-FE0F", "google": "1F5E3-FE0F", "twitter": "1F5E3-FE0F" },
            "shortcode": "speaking_head_in_silhouette",
            "category": "people"
        },
        {
            "name": "bust_in_silhouette",
            "unicode": { "apple": "1F464", "google": "1F464", "twitter": "1F464" },
            "shortcode": "bust_in_silhouette",
            "category": "people"
        },
        {
            "name": "busts_in_silhouette",
            "unicode": { "apple": "1F465", "google": "1F465", "twitter": "1F465" },
            "shortcode": "busts_in_silhouette",
            "category": "people"
        },
        {
            "name": "fencer",
            "unicode": { "apple": "1F93A", "google": "1F93A", "twitter": "1F93A" },
            "shortcode": "fencer",
            "category": "people"
        },
        {
            "name": "horse_racing",
            "unicode": { "apple": "1F3C7", "google": "1F3C7", "twitter": "1F3C7" },
            "shortcode": "horse_racing",
            "category": "people"
        },
        {
            "name": "skier",
            "unicode": { "apple": "26F7-FE0F", "google": "26F7-FE0F", "twitter": "26F7-FE0F" },
            "shortcode": "skier",
            "category": "people"
        },
        {
            "name": "snowboarder",
            "unicode": { "apple": "1F3C2", "google": "1F3C2", "twitter": "1F3C2" },
            "shortcode": "snowboarder",
            "category": "people"
        },
        {
            "name": "golfer",
            "unicode": { "apple": "1F3CC-FE0F", "google": "1F3CC-FE0F", "twitter": "1F3CC-FE0F" },
            "shortcode": "golfer",
            "category": "people"
        },
        {
            "name": "man-golfing",
            "unicode": {
                "apple": "1F3CC-FE0F-200D-2642-FE0F",
                "google": "1F3CC-FE0F-200D-2642-FE0F",
                "twitter": "1F3CC-FE0F-200D-2642-FE0F"
            },
            "shortcode": "man-golfing",
            "category": "people"
        },
        {
            "name": "woman-golfing",
            "unicode": {
                "apple": "1F3CC-FE0F-200D-2640-FE0F",
                "google": "1F3CC-FE0F-200D-2640-FE0F",
                "twitter": "1F3CC-FE0F-200D-2640-FE0F"
            },
            "shortcode": "woman-golfing",
            "category": "people"
        },
        {
            "name": "surfer",
            "unicode": { "apple": "1F3C4", "google": "1F3C4", "twitter": "1F3C4" },
            "shortcode": "surfer",
            "category": "people"
        },
        {
            "name": "man-surfing",
            "unicode": {
                "apple": "1F3C4-200D-2642-FE0F",
                "google": "1F3C4-200D-2642-FE0F",
                "twitter": "1F3C4-200D-2642-FE0F"
            },
            "shortcode": "man-surfing",
            "category": "people"
        },
        {
            "name": "woman-surfing",
            "unicode": {
                "apple": "1F3C4-200D-2640-FE0F",
                "google": "1F3C4-200D-2640-FE0F",
                "twitter": "1F3C4-200D-2640-FE0F"
            },
            "shortcode": "woman-surfing",
            "category": "people"
        },
        {
            "name": "rowboat",
            "unicode": { "apple": "1F6A3", "google": "1F6A3", "twitter": "1F6A3" },
            "shortcode": "rowboat",
            "category": "people"
        },
        {
            "name": "man-rowing-boat",
            "unicode": {
                "apple": "1F6A3-200D-2642-FE0F",
                "google": "1F6A3-200D-2642-FE0F",
                "twitter": "1F6A3-200D-2642-FE0F"
            },
            "shortcode": "man-rowing-boat",
            "category": "people"
        },
        {
            "name": "woman-rowing-boat",
            "unicode": {
                "apple": "1F6A3-200D-2640-FE0F",
                "google": "1F6A3-200D-2640-FE0F",
                "twitter": "1F6A3-200D-2640-FE0F"
            },
            "shortcode": "woman-rowing-boat",
            "category": "people"
        },
        {
            "name": "swimmer",
            "unicode": { "apple": "1F3CA", "google": "1F3CA", "twitter": "1F3CA" },
            "shortcode": "swimmer",
            "category": "people"
        },
        {
            "name": "man-swimming",
            "unicode": {
                "apple": "1F3CA-200D-2642-FE0F",
                "google": "1F3CA-200D-2642-FE0F",
                "twitter": "1F3CA-200D-2642-FE0F"
            },
            "shortcode": "man-swimming",
            "category": "people"
        },
        {
            "name": "woman-swimming",
            "unicode": {
                "apple": "1F3CA-200D-2640-FE0F",
                "google": "1F3CA-200D-2640-FE0F",
                "twitter": "1F3CA-200D-2640-FE0F"
            },
            "shortcode": "woman-swimming",
            "category": "people"
        },
        {
            "name": "person_with_ball",
            "unicode": { "apple": "26F9-FE0F", "google": "26F9-FE0F", "twitter": "26F9-FE0F" },
            "shortcode": "person_with_ball",
            "category": "people"
        },
        {
            "name": "man-bouncing-ball",
            "unicode": {
                "apple": "26F9-FE0F-200D-2642-FE0F",
                "google": "26F9-FE0F-200D-2642-FE0F",
                "twitter": "26F9-FE0F-200D-2642-FE0F"
            },
            "shortcode": "man-bouncing-ball",
            "category": "people"
        },
        {
            "name": "woman-bouncing-ball",
            "unicode": {
                "apple": "26F9-FE0F-200D-2640-FE0F",
                "google": "26F9-FE0F-200D-2640-FE0F",
                "twitter": "26F9-FE0F-200D-2640-FE0F"
            },
            "shortcode": "woman-bouncing-ball",
            "category": "people"
        },
        {
            "name": "weight_lifter",
            "unicode": { "apple": "1F3CB-FE0F", "google": "1F3CB-FE0F", "twitter": "1F3CB-FE0F" },
            "shortcode": "weight_lifter",
            "category": "people"
        },
        {
            "name": "man-lifting-weights",
            "unicode": {
                "apple": "1F3CB-FE0F-200D-2642-FE0F",
                "google": "1F3CB-FE0F-200D-2642-FE0F",
                "twitter": "1F3CB-FE0F-200D-2642-FE0F"
            },
            "shortcode": "man-lifting-weights",
            "category": "people"
        },
        {
            "name": "woman-lifting-weights",
            "unicode": {
                "apple": "1F3CB-FE0F-200D-2640-FE0F",
                "google": "1F3CB-FE0F-200D-2640-FE0F",
                "twitter": "1F3CB-FE0F-200D-2640-FE0F"
            },
            "shortcode": "woman-lifting-weights",
            "category": "people"
        },
        {
            "name": "bicyclist",
            "unicode": { "apple": "1F6B4", "google": "1F6B4", "twitter": "1F6B4" },
            "shortcode": "bicyclist",
            "category": "people"
        },
        {
            "name": "man-biking",
            "unicode": {
                "apple": "1F6B4-200D-2642-FE0F",
                "google": "1F6B4-200D-2642-FE0F",
                "twitter": "1F6B4-200D-2642-FE0F"
            },
            "shortcode": "man-biking",
            "category": "people"
        },
        {
            "name": "woman-biking",
            "unicode": {
                "apple": "1F6B4-200D-2640-FE0F",
                "google": "1F6B4-200D-2640-FE0F",
                "twitter": "1F6B4-200D-2640-FE0F"
            },
            "shortcode": "woman-biking",
            "category": "people"
        },
        {
            "name": "mountain_bicyclist",
            "unicode": { "apple": "1F6B5", "google": "1F6B5", "twitter": "1F6B5" },
            "shortcode": "mountain_bicyclist",
            "category": "people"
        },
        {
            "name": "man-mountain-biking",
            "unicode": {
                "apple": "1F6B5-200D-2642-FE0F",
                "google": "1F6B5-200D-2642-FE0F",
                "twitter": "1F6B5-200D-2642-FE0F"
            },
            "shortcode": "man-mountain-biking",
            "category": "people"
        },
        {
            "name": "woman-mountain-biking",
            "unicode": {
                "apple": "1F6B5-200D-2640-FE0F",
                "google": "1F6B5-200D-2640-FE0F",
                "twitter": "1F6B5-200D-2640-FE0F"
            },
            "shortcode": "woman-mountain-biking",
            "category": "people"
        },
        {
            "name": "racing_car",
            "unicode": { "apple": "1F3CE-FE0F", "google": "1F3CE-FE0F", "twitter": "1F3CE-FE0F" },
            "shortcode": "racing_car",
            "category": "people"
        },
        {
            "name": "racing_motorcycle",
            "unicode": { "apple": "1F3CD-FE0F", "google": "1F3CD-FE0F", "twitter": "1F3CD-FE0F" },
            "shortcode": "racing_motorcycle",
            "category": "people"
        },
        {
            "name": "person_doing_cartwheel",
            "unicode": { "apple": "1F938", "google": "1F938", "twitter": "1F938" },
            "shortcode": "person_doing_cartwheel",
            "category": "people"
        },
        {
            "name": "man-cartwheeling",
            "unicode": {
                "apple": "1F938-200D-2642-FE0F",
                "google": "1F938-200D-2642-FE0F",
                "twitter": "1F938-200D-2642-FE0F"
            },
            "shortcode": "man-cartwheeling",
            "category": "people"
        },
        {
            "name": "woman-cartwheeling",
            "unicode": {
                "apple": "1F938-200D-2640-FE0F",
                "google": "1F938-200D-2640-FE0F",
                "twitter": "1F938-200D-2640-FE0F"
            },
            "shortcode": "woman-cartwheeling",
            "category": "people"
        },
        {
            "name": "wrestlers",
            "unicode": { "apple": "1F93C", "google": "1F93C", "twitter": "1F93C" },
            "shortcode": "wrestlers",
            "category": "people"
        },
        {
            "name": "man-wrestling",
            "unicode": {
                "apple": "1F93C-200D-2642-FE0F",
                "google": "1F93C-200D-2642-FE0F",
                "twitter": "1F93C-200D-2642-FE0F"
            },
            "shortcode": "man-wrestling",
            "category": "people"
        },
        {
            "name": "woman-wrestling",
            "unicode": {
                "apple": "1F93C-200D-2640-FE0F",
                "google": "1F93C-200D-2640-FE0F",
                "twitter": "1F93C-200D-2640-FE0F"
            },
            "shortcode": "woman-wrestling",
            "category": "people"
        },
        {
            "name": "water_polo",
            "unicode": { "apple": "1F93D", "google": "1F93D", "twitter": "1F93D" },
            "shortcode": "water_polo",
            "category": "people"
        },
        {
            "name": "man-playing-water-polo",
            "unicode": {
                "apple": "1F93D-200D-2642-FE0F",
                "google": "1F93D-200D-2642-FE0F",
                "twitter": "1F93D-200D-2642-FE0F"
            },
            "shortcode": "man-playing-water-polo",
            "category": "people"
        },
        {
            "name": "woman-playing-water-polo",
            "unicode": {
                "apple": "1F93D-200D-2640-FE0F",
                "google": "1F93D-200D-2640-FE0F",
                "twitter": "1F93D-200D-2640-FE0F"
            },
            "shortcode": "woman-playing-water-polo",
            "category": "people"
        },
        {
            "name": "handball",
            "unicode": { "apple": "1F93E", "google": "1F93E", "twitter": "1F93E" },
            "shortcode": "handball",
            "category": "people"
        },
        {
            "name": "man-playing-handball",
            "unicode": {
                "apple": "1F93E-200D-2642-FE0F",
                "google": "1F93E-200D-2642-FE0F",
                "twitter": "1F93E-200D-2642-FE0F"
            },
            "shortcode": "man-playing-handball",
            "category": "people"
        },
        {
            "name": "woman-playing-handball",
            "unicode": {
                "apple": "1F93E-200D-2640-FE0F",
                "google": "1F93E-200D-2640-FE0F",
                "twitter": "1F93E-200D-2640-FE0F"
            },
            "shortcode": "woman-playing-handball",
            "category": "people"
        },
        {
            "name": "juggling",
            "unicode": { "apple": "1F939", "google": "1F939", "twitter": "1F939" },
            "shortcode": "juggling",
            "category": "people"
        },
        {
            "name": "man-juggling",
            "unicode": {
                "apple": "1F939-200D-2642-FE0F",
                "google": "1F939-200D-2642-FE0F",
                "twitter": "1F939-200D-2642-FE0F"
            },
            "shortcode": "man-juggling",
            "category": "people"
        },
        {
            "name": "woman-juggling",
            "unicode": {
                "apple": "1F939-200D-2640-FE0F",
                "google": "1F939-200D-2640-FE0F",
                "twitter": "1F939-200D-2640-FE0F"
            },
            "shortcode": "woman-juggling",
            "category": "people"
        },
        {
            "name": "couple",
            "unicode": { "apple": "1F46B", "google": "1F46B", "twitter": "1F46B" },
            "shortcode": "couple",
            "category": "people"
        },
        {
            "name": "two_men_holding_hands",
            "unicode": { "apple": "1F46C", "google": "1F46C", "twitter": "1F46C" },
            "shortcode": "two_men_holding_hands",
            "category": "people"
        },
        {
            "name": "two_women_holding_hands",
            "unicode": { "apple": "1F46D", "google": "1F46D", "twitter": "1F46D" },
            "shortcode": "two_women_holding_hands",
            "category": "people"
        },
        {
            "name": "couplekiss",
            "unicode": { "apple": "1F48F", "google": "1F48F", "twitter": "1F48F" },
            "shortcode": "couplekiss",
            "category": "people"
        },
        {
            "name": "woman-kiss-man",
            "unicode": {
                "apple": "1F469-200D-2764-FE0F-200D-1F48B-200D-1F468",
                "google": "1F469-200D-2764-FE0F-200D-1F48B-200D-1F468",
                "twitter": "1F469-200D-2764-FE0F-200D-1F48B-200D-1F468"
            },
            "shortcode": "woman-kiss-man",
            "category": "people"
        },
        {
            "name": "man-kiss-man",
            "unicode": {
                "apple": "1F468-200D-2764-FE0F-200D-1F48B-200D-1F468",
                "google": "1F468-200D-2764-FE0F-200D-1F48B-200D-1F468",
                "twitter": "1F468-200D-2764-FE0F-200D-1F48B-200D-1F468"
            },
            "shortcode": "man-kiss-man",
            "category": "people"
        },
        {
            "name": "woman-kiss-woman",
            "unicode": {
                "apple": "1F469-200D-2764-FE0F-200D-1F48B-200D-1F469",
                "google": "1F469-200D-2764-FE0F-200D-1F48B-200D-1F469",
                "twitter": "1F469-200D-2764-FE0F-200D-1F48B-200D-1F469"
            },
            "shortcode": "woman-kiss-woman",
            "category": "people"
        },
        {
            "name": "couple_with_heart",
            "unicode": { "apple": "1F491", "google": "1F491", "twitter": "1F491" },
            "shortcode": "couple_with_heart",
            "category": "people"
        },
        {
            "name": "woman-heart-man",
            "unicode": {
                "apple": "1F469-200D-2764-FE0F-200D-1F468",
                "google": "1F469-200D-2764-FE0F-200D-1F468",
                "twitter": "1F469-200D-2764-FE0F-200D-1F468"
            },
            "shortcode": "woman-heart-man",
            "category": "people"
        },
        {
            "name": "man-heart-man",
            "unicode": {
                "apple": "1F468-200D-2764-FE0F-200D-1F468",
                "google": "1F468-200D-2764-FE0F-200D-1F468",
                "twitter": "1F468-200D-2764-FE0F-200D-1F468"
            },
            "shortcode": "man-heart-man",
            "category": "people"
        },
        {
            "name": "woman-heart-woman",
            "unicode": {
                "apple": "1F469-200D-2764-FE0F-200D-1F469",
                "google": "1F469-200D-2764-FE0F-200D-1F469",
                "twitter": "1F469-200D-2764-FE0F-200D-1F469"
            },
            "shortcode": "woman-heart-woman",
            "category": "people"
        },
        {
            "name": "family",
            "unicode": { "apple": "1F46A", "google": "1F46A", "twitter": "1F46A" },
            "shortcode": "family",
            "category": "people"
        },
        {
            "name": "man-woman-boy",
            "unicode": {
                "apple": "1F468-200D-1F469-200D-1F466",
                "google": "1F468-200D-1F469-200D-1F466",
                "twitter": "1F468-200D-1F469-200D-1F466"
            },
            "shortcode": "man-woman-boy",
            "category": "people"
        },
        {
            "name": "man-woman-girl",
            "unicode": {
                "apple": "1F468-200D-1F469-200D-1F467",
                "google": "1F468-200D-1F469-200D-1F467",
                "twitter": "1F468-200D-1F469-200D-1F467"
            },
            "shortcode": "man-woman-girl",
            "category": "people"
        },
        {
            "name": "man-woman-girl-boy",
            "unicode": {
                "apple": "1F468-200D-1F469-200D-1F467-200D-1F466",
                "google": "1F468-200D-1F469-200D-1F467-200D-1F466",
                "twitter": "1F468-200D-1F469-200D-1F467-200D-1F466"
            },
            "shortcode": "man-woman-girl-boy",
            "category": "people"
        },
        {
            "name": "man-woman-boy-boy",
            "unicode": {
                "apple": "1F468-200D-1F469-200D-1F466-200D-1F466",
                "google": "1F468-200D-1F469-200D-1F466-200D-1F466",
                "twitter": "1F468-200D-1F469-200D-1F466-200D-1F466"
            },
            "shortcode": "man-woman-boy-boy",
            "category": "people"
        },
        {
            "name": "man-woman-girl-girl",
            "unicode": {
                "apple": "1F468-200D-1F469-200D-1F467-200D-1F467",
                "google": "1F468-200D-1F469-200D-1F467-200D-1F467",
                "twitter": "1F468-200D-1F469-200D-1F467-200D-1F467"
            },
            "shortcode": "man-woman-girl-girl",
            "category": "people"
        },
        {
            "name": "man-man-boy",
            "unicode": {
                "apple": "1F468-200D-1F468-200D-1F466",
                "google": "1F468-200D-1F468-200D-1F466",
                "twitter": "1F468-200D-1F468-200D-1F466"
            },
            "shortcode": "man-man-boy",
            "category": "people"
        },
        {
            "name": "man-man-girl",
            "unicode": {
                "apple": "1F468-200D-1F468-200D-1F467",
                "google": "1F468-200D-1F468-200D-1F467",
                "twitter": "1F468-200D-1F468-200D-1F467"
            },
            "shortcode": "man-man-girl",
            "category": "people"
        },
        {
            "name": "man-man-girl-boy",
            "unicode": {
                "apple": "1F468-200D-1F468-200D-1F467-200D-1F466",
                "google": "1F468-200D-1F468-200D-1F467-200D-1F466",
                "twitter": "1F468-200D-1F468-200D-1F467-200D-1F466"
            },
            "shortcode": "man-man-girl-boy",
            "category": "people"
        },
        {
            "name": "man-man-boy-boy",
            "unicode": {
                "apple": "1F468-200D-1F468-200D-1F466-200D-1F466",
                "google": "1F468-200D-1F468-200D-1F466-200D-1F466",
                "twitter": "1F468-200D-1F468-200D-1F466-200D-1F466"
            },
            "shortcode": "man-man-boy-boy",
            "category": "people"
        },
        {
            "name": "man-man-girl-girl",
            "unicode": {
                "apple": "1F468-200D-1F468-200D-1F467-200D-1F467",
                "google": "1F468-200D-1F468-200D-1F467-200D-1F467",
                "twitter": "1F468-200D-1F468-200D-1F467-200D-1F467"
            },
            "shortcode": "man-man-girl-girl",
            "category": "people"
        },
        {
            "name": "woman-woman-boy",
            "unicode": {
                "apple": "1F469-200D-1F469-200D-1F466",
                "google": "1F469-200D-1F469-200D-1F466",
                "twitter": "1F469-200D-1F469-200D-1F466"
            },
            "shortcode": "woman-woman-boy",
            "category": "people"
        },
        {
            "name": "woman-woman-girl",
            "unicode": {
                "apple": "1F469-200D-1F469-200D-1F467",
                "google": "1F469-200D-1F469-200D-1F467",
                "twitter": "1F469-200D-1F469-200D-1F467"
            },
            "shortcode": "woman-woman-girl",
            "category": "people"
        },
        {
            "name": "woman-woman-girl-boy",
            "unicode": {
                "apple": "1F469-200D-1F469-200D-1F467-200D-1F466",
                "google": "1F469-200D-1F469-200D-1F467-200D-1F466",
                "twitter": "1F469-200D-1F469-200D-1F467-200D-1F466"
            },
            "shortcode": "woman-woman-girl-boy",
            "category": "people"
        },
        {
            "name": "woman-woman-boy-boy",
            "unicode": {
                "apple": "1F469-200D-1F469-200D-1F466-200D-1F466",
                "google": "1F469-200D-1F469-200D-1F466-200D-1F466",
                "twitter": "1F469-200D-1F469-200D-1F466-200D-1F466"
            },
            "shortcode": "woman-woman-boy-boy",
            "category": "people"
        },
        {
            "name": "woman-woman-girl-girl",
            "unicode": {
                "apple": "1F469-200D-1F469-200D-1F467-200D-1F467",
                "google": "1F469-200D-1F469-200D-1F467-200D-1F467",
                "twitter": "1F469-200D-1F469-200D-1F467-200D-1F467"
            },
            "shortcode": "woman-woman-girl-girl",
            "category": "people"
        },
        {
            "name": "man-boy",
            "unicode": { "apple": "1F468-200D-1F466", "google": "1F468-200D-1F466", "twitter": "1F468-200D-1F466" },
            "shortcode": "man-boy",
            "category": "people"
        },
        {
            "name": "man-boy-boy",
            "unicode": {
                "apple": "1F468-200D-1F466-200D-1F466",
                "google": "1F468-200D-1F466-200D-1F466",
                "twitter": "1F468-200D-1F466-200D-1F466"
            },
            "shortcode": "man-boy-boy",
            "category": "people"
        },
        {
            "name": "man-girl",
            "unicode": { "apple": "1F468-200D-1F467", "google": "1F468-200D-1F467", "twitter": "1F468-200D-1F467" },
            "shortcode": "man-girl",
            "category": "people"
        },
        {
            "name": "man-girl-boy",
            "unicode": {
                "apple": "1F468-200D-1F467-200D-1F466",
                "google": "1F468-200D-1F467-200D-1F466",
                "twitter": "1F468-200D-1F467-200D-1F466"
            },
            "shortcode": "man-girl-boy",
            "category": "people"
        },
        {
            "name": "man-girl-girl",
            "unicode": {
                "apple": "1F468-200D-1F467-200D-1F467",
                "google": "1F468-200D-1F467-200D-1F467",
                "twitter": "1F468-200D-1F467-200D-1F467"
            },
            "shortcode": "man-girl-girl",
            "category": "people"
        },
        {
            "name": "woman-boy",
            "unicode": { "apple": "1F469-200D-1F466", "google": "1F469-200D-1F466", "twitter": "1F469-200D-1F466" },
            "shortcode": "woman-boy",
            "category": "people"
        },
        {
            "name": "woman-boy-boy",
            "unicode": {
                "apple": "1F469-200D-1F466-200D-1F466",
                "google": "1F469-200D-1F466-200D-1F466",
                "twitter": "1F469-200D-1F466-200D-1F466"
            },
            "shortcode": "woman-boy-boy",
            "category": "people"
        },
        {
            "name": "woman-girl",
            "unicode": { "apple": "1F469-200D-1F467", "google": "1F469-200D-1F467", "twitter": "1F469-200D-1F467" },
            "shortcode": "woman-girl",
            "category": "people"
        },
        {
            "name": "woman-girl-boy",
            "unicode": {
                "apple": "1F469-200D-1F467-200D-1F466",
                "google": "1F469-200D-1F467-200D-1F466",
                "twitter": "1F469-200D-1F467-200D-1F466"
            },
            "shortcode": "woman-girl-boy",
            "category": "people"
        },
        {
            "name": "woman-girl-girl",
            "unicode": {
                "apple": "1F469-200D-1F467-200D-1F467",
                "google": "1F469-200D-1F467-200D-1F467",
                "twitter": "1F469-200D-1F467-200D-1F467"
            },
            "shortcode": "woman-girl-girl",
            "category": "people"
        },
        {
            "name": "selfie",
            "unicode": { "apple": "1F933", "google": "1F933", "twitter": "1F933" },
            "shortcode": "selfie",
            "category": "people"
        },
        {
            "name": "muscle",
            "unicode": { "apple": "1F4AA", "google": "1F4AA", "twitter": "1F4AA" },
            "shortcode": "muscle",
            "category": "people"
        },
        {
            "name": "point_left",
            "unicode": { "apple": "1F448", "google": "1F448", "twitter": "1F448" },
            "shortcode": "point_left",
            "category": "people"
        },
        {
            "name": "point_right",
            "unicode": { "apple": "1F449", "google": "1F449", "twitter": "1F449" },
            "shortcode": "point_right",
            "category": "people"
        },
        {
            "name": "point_up",
            "unicode": { "apple": "261D-FE0F", "google": "261D-FE0F", "twitter": "261D-FE0F" },
            "shortcode": "point_up",
            "category": "people"
        },
        {
            "name": "point_up_2",
            "unicode": { "apple": "1F446", "google": "1F446", "twitter": "1F446" },
            "shortcode": "point_up_2",
            "category": "people"
        },
        {
            "name": "middle_finger",
            "unicode": { "apple": "1F595", "google": "1F595", "twitter": "1F595" },
            "shortcode": "middle_finger",
            "category": "people"
        },
        {
            "name": "point_down",
            "unicode": { "apple": "1F447", "google": "1F447", "twitter": "1F447" },
            "shortcode": "point_down",
            "category": "people"
        },
        {
            "name": "v",
            "unicode": { "apple": "270C-FE0F", "google": "270C-FE0F", "twitter": "270C-FE0F" },
            "shortcode": "v",
            "category": "people"
        },
        {
            "name": "crossed_fingers",
            "unicode": { "apple": "1F91E", "google": "1F91E", "twitter": "1F91E" },
            "shortcode": "crossed_fingers",
            "category": "people"
        },
        {
            "name": "spock-hand",
            "unicode": { "apple": "1F596", "google": "1F596", "twitter": "1F596" },
            "shortcode": "spock-hand",
            "category": "people"
        },
        {
            "name": "the_horns",
            "unicode": { "apple": "1F918", "google": "1F918", "twitter": "1F918" },
            "shortcode": "the_horns",
            "category": "people"
        },
        {
            "name": "call_me_hand",
            "unicode": { "apple": "1F919", "google": "1F919", "twitter": "1F919" },
            "shortcode": "call_me_hand",
            "category": "people"
        },
        {
            "name": "raised_hand_with_fingers_splayed",
            "unicode": { "apple": "1F590-FE0F", "google": "1F590-FE0F", "twitter": "1F590-FE0F" },
            "shortcode": "raised_hand_with_fingers_splayed",
            "category": "people"
        },
        {
            "name": "hand",
            "unicode": { "apple": "270B", "google": "270B", "twitter": "270B" },
            "shortcode": "hand",
            "category": "people"
        },
        {
            "name": "ok_hand",
            "unicode": { "apple": "1F44C", "google": "1F44C", "twitter": "1F44C" },
            "shortcode": "ok_hand",
            "category": "people"
        },
        {
            "name": "+1",
            "unicode": { "apple": "1F44D", "google": "1F44D", "twitter": "1F44D" },
            "shortcode": "+1",
            "category": "people"
        },
        {
            "name": "-1",
            "unicode": { "apple": "1F44E", "google": "1F44E", "twitter": "1F44E" },
            "shortcode": "-1",
            "category": "people"
        },
        {
            "name": "fist",
            "unicode": { "apple": "270A", "google": "270A", "twitter": "270A" },
            "shortcode": "fist",
            "category": "people"
        },
        {
            "name": "facepunch",
            "unicode": { "apple": "1F44A", "google": "1F44A", "twitter": "1F44A" },
            "shortcode": "facepunch",
            "category": "people"
        },
        {
            "name": "left-facing_fist",
            "unicode": { "apple": "1F91B", "google": "1F91B", "twitter": "1F91B" },
            "shortcode": "left-facing_fist",
            "category": "people"
        },
        {
            "name": "right-facing_fist",
            "unicode": { "apple": "1F91C", "google": "1F91C", "twitter": "1F91C" },
            "shortcode": "right-facing_fist",
            "category": "people"
        },
        {
            "name": "raised_back_of_hand",
            "unicode": { "apple": "1F91A", "google": "1F91A", "twitter": "1F91A" },
            "shortcode": "raised_back_of_hand",
            "category": "people"
        },
        {
            "name": "wave",
            "unicode": { "apple": "1F44B", "google": "1F44B", "twitter": "1F44B" },
            "shortcode": "wave",
            "category": "people"
        },
        {
            "name": "i_love_you_hand_sign",
            "unicode": { "apple": "1F91F", "google": "1F91F", "twitter": "1F91F" },
            "shortcode": "i_love_you_hand_sign",
            "category": "people"
        },
        {
            "name": "writing_hand",
            "unicode": { "apple": "270D-FE0F", "google": "270D-FE0F", "twitter": "270D-FE0F" },
            "shortcode": "writing_hand",
            "category": "people"
        },
        {
            "name": "clap",
            "unicode": { "apple": "1F44F", "google": "1F44F", "twitter": "1F44F" },
            "shortcode": "clap",
            "category": "people"
        },
        {
            "name": "open_hands",
            "unicode": { "apple": "1F450", "google": "1F450", "twitter": "1F450" },
            "shortcode": "open_hands",
            "category": "people"
        },
        {
            "name": "raised_hands",
            "unicode": { "apple": "1F64C", "google": "1F64C", "twitter": "1F64C" },
            "shortcode": "raised_hands",
            "category": "people"
        },
        {
            "name": "palms_up_together",
            "unicode": { "apple": "1F932", "google": "1F932", "twitter": "1F932" },
            "shortcode": "palms_up_together",
            "category": "people"
        },
        {
            "name": "pray",
            "unicode": { "apple": "1F64F", "google": "1F64F", "twitter": "1F64F" },
            "shortcode": "pray",
            "category": "people"
        },
        {
            "name": "handshake",
            "unicode": { "apple": "1F91D", "google": "1F91D", "twitter": "1F91D" },
            "shortcode": "handshake",
            "category": "people"
        },
        {
            "name": "nail_care",
            "unicode": { "apple": "1F485", "google": "1F485", "twitter": "1F485" },
            "shortcode": "nail_care",
            "category": "people"
        },
        {
            "name": "ear",
            "unicode": { "apple": "1F442", "google": "1F442", "twitter": "1F442" },
            "shortcode": "ear",
            "category": "people"
        },
        {
            "name": "nose",
            "unicode": { "apple": "1F443", "google": "1F443", "twitter": "1F443" },
            "shortcode": "nose",
            "category": "people"
        },
        {
            "name": "footprints",
            "unicode": { "apple": "1F463", "google": "1F463", "twitter": "1F463" },
            "shortcode": "footprints",
            "category": "people"
        },
        {
            "name": "eyes",
            "unicode": { "apple": "1F440", "google": "1F440", "twitter": "1F440" },
            "shortcode": "eyes",
            "category": "people"
        },
        {
            "name": "eye",
            "unicode": { "apple": "1F441-FE0F", "google": "1F441-FE0F", "twitter": "1F441-FE0F" },
            "shortcode": "eye",
            "category": "people"
        },
        {
            "name": "eye-in-speech-bubble",
            "unicode": {
                "apple": "1F441-FE0F-200D-1F5E8-FE0F",
                "google": "1F441-FE0F-200D-1F5E8-FE0F",
                "twitter": "1F441-FE0F-200D-1F5E8-FE0F"
            },
            "shortcode": "eye-in-speech-bubble",
            "category": "people"
        },
        {
            "name": "brain",
            "unicode": { "apple": "1F9E0", "google": "1F9E0", "twitter": "1F9E0" },
            "shortcode": "brain",
            "category": "people"
        },
        {
            "name": "tongue",
            "unicode": { "apple": "1F445", "google": "1F445", "twitter": "1F445" },
            "shortcode": "tongue",
            "category": "people"
        },
        {
            "name": "lips",
            "unicode": { "apple": "1F444", "google": "1F444", "twitter": "1F444" },
            "shortcode": "lips",
            "category": "people"
        },
        {
            "name": "kiss",
            "unicode": { "apple": "1F48B", "google": "1F48B", "twitter": "1F48B" },
            "shortcode": "kiss",
            "category": "people"
        },
        {
            "name": "cupid",
            "unicode": { "apple": "1F498", "google": "1F498", "twitter": "1F498" },
            "shortcode": "cupid",
            "category": "people"
        },
        {
            "name": "heart",
            "unicode": { "apple": "2764-FE0F", "google": "2764-FE0F", "twitter": "2764-FE0F" },
            "shortcode": "heart",
            "category": "people"
        },
        {
            "name": "heartbeat",
            "unicode": { "apple": "1F493", "google": "1F493", "twitter": "1F493" },
            "shortcode": "heartbeat",
            "category": "people"
        },
        {
            "name": "broken_heart",
            "unicode": { "apple": "1F494", "google": "1F494", "twitter": "1F494" },
            "shortcode": "broken_heart",
            "category": "people"
        },
        {
            "name": "two_hearts",
            "unicode": { "apple": "1F495", "google": "1F495", "twitter": "1F495" },
            "shortcode": "two_hearts",
            "category": "people"
        },
        {
            "name": "sparkling_heart",
            "unicode": { "apple": "1F496", "google": "1F496", "twitter": "1F496" },
            "shortcode": "sparkling_heart",
            "category": "people"
        },
        {
            "name": "heartpulse",
            "unicode": { "apple": "1F497", "google": "1F497", "twitter": "1F497" },
            "shortcode": "heartpulse",
            "category": "people"
        },
        {
            "name": "blue_heart",
            "unicode": { "apple": "1F499", "google": "1F499", "twitter": "1F499" },
            "shortcode": "blue_heart",
            "category": "people"
        },
        {
            "name": "green_heart",
            "unicode": { "apple": "1F49A", "google": "1F49A", "twitter": "1F49A" },
            "shortcode": "green_heart",
            "category": "people"
        },
        {
            "name": "yellow_heart",
            "unicode": { "apple": "1F49B", "google": "1F49B", "twitter": "1F49B" },
            "shortcode": "yellow_heart",
            "category": "people"
        },
        {
            "name": "orange_heart",
            "unicode": { "apple": "1F9E1", "google": "1F9E1", "twitter": "1F9E1" },
            "shortcode": "orange_heart",
            "category": "people"
        },
        {
            "name": "purple_heart",
            "unicode": { "apple": "1F49C", "google": "1F49C", "twitter": "1F49C" },
            "shortcode": "purple_heart",
            "category": "people"
        },
        {
            "name": "black_heart",
            "unicode": { "apple": "1F5A4", "google": "1F5A4", "twitter": "1F5A4" },
            "shortcode": "black_heart",
            "category": "people"
        },
        {
            "name": "gift_heart",
            "unicode": { "apple": "1F49D", "google": "1F49D", "twitter": "1F49D" },
            "shortcode": "gift_heart",
            "category": "people"
        },
        {
            "name": "revolving_hearts",
            "unicode": { "apple": "1F49E", "google": "1F49E", "twitter": "1F49E" },
            "shortcode": "revolving_hearts",
            "category": "people"
        },
        {
            "name": "heart_decoration",
            "unicode": { "apple": "1F49F", "google": "1F49F", "twitter": "1F49F" },
            "shortcode": "heart_decoration",
            "category": "people"
        },
        {
            "name": "heavy_heart_exclamation_mark_ornament",
            "unicode": { "apple": "2763-FE0F", "google": "2763-FE0F", "twitter": "2763-FE0F" },
            "shortcode": "heavy_heart_exclamation_mark_ornament",
            "category": "people"
        },
        {
            "name": "love_letter",
            "unicode": { "apple": "1F48C", "google": "1F48C", "twitter": "1F48C" },
            "shortcode": "love_letter",
            "category": "people"
        },
        {
            "name": "zzz",
            "unicode": { "apple": "1F4A4", "google": "1F4A4", "twitter": "1F4A4" },
            "shortcode": "zzz",
            "category": "people"
        },
        {
            "name": "anger",
            "unicode": { "apple": "1F4A2", "google": "1F4A2", "twitter": "1F4A2" },
            "shortcode": "anger",
            "category": "people"
        },
        {
            "name": "bomb",
            "unicode": { "apple": "1F4A3", "google": "1F4A3", "twitter": "1F4A3" },
            "shortcode": "bomb",
            "category": "people"
        },
        {
            "name": "boom",
            "unicode": { "apple": "1F4A5", "google": "1F4A5", "twitter": "1F4A5" },
            "shortcode": "boom",
            "category": "people"
        },
        {
            "name": "sweat_drops",
            "unicode": { "apple": "1F4A6", "google": "1F4A6", "twitter": "1F4A6" },
            "shortcode": "sweat_drops",
            "category": "people"
        },
        {
            "name": "dash",
            "unicode": { "apple": "1F4A8", "google": "1F4A8", "twitter": "1F4A8" },
            "shortcode": "dash",
            "category": "people"
        },
        {
            "name": "dizzy",
            "unicode": { "apple": "1F4AB", "google": "1F4AB", "twitter": "1F4AB" },
            "shortcode": "dizzy",
            "category": "people"
        },
        {
            "name": "speech_balloon",
            "unicode": { "apple": "1F4AC", "google": "1F4AC", "twitter": "1F4AC" },
            "shortcode": "speech_balloon",
            "category": "people"
        },
        {
            "name": "left_speech_bubble",
            "unicode": { "apple": "1F5E8-FE0F", "google": "1F5E8-FE0F", "twitter": "1F5E8-FE0F" },
            "shortcode": "left_speech_bubble",
            "category": "people"
        },
        {
            "name": "right_anger_bubble",
            "unicode": { "apple": "1F5EF-FE0F", "google": "1F5EF-FE0F", "twitter": "1F5EF-FE0F" },
            "shortcode": "right_anger_bubble",
            "category": "people"
        },
        {
            "name": "thought_balloon",
            "unicode": { "apple": "1F4AD", "google": "1F4AD", "twitter": "1F4AD" },
            "shortcode": "thought_balloon",
            "category": "people"
        },
        {
            "name": "hole",
            "unicode": { "apple": "1F573-FE0F", "google": "1F573-FE0F", "twitter": "1F573-FE0F" },
            "shortcode": "hole",
            "category": "people"
        },
        {
            "name": "eyeglasses",
            "unicode": { "apple": "1F453", "google": "1F453", "twitter": "1F453" },
            "shortcode": "eyeglasses",
            "category": "people"
        },
        {
            "name": "dark_sunglasses",
            "unicode": { "apple": "1F576-FE0F", "google": "1F576-FE0F", "twitter": "1F576-FE0F" },
            "shortcode": "dark_sunglasses",
            "category": "people"
        },
        {
            "name": "necktie",
            "unicode": { "apple": "1F454", "google": "1F454", "twitter": "1F454" },
            "shortcode": "necktie",
            "category": "people"
        },
        {
            "name": "shirt",
            "unicode": { "apple": "1F455", "google": "1F455", "twitter": "1F455" },
            "shortcode": "shirt",
            "category": "people"
        },
        {
            "name": "jeans",
            "unicode": { "apple": "1F456", "google": "1F456", "twitter": "1F456" },
            "shortcode": "jeans",
            "category": "people"
        },
        {
            "name": "scarf",
            "unicode": { "apple": "1F9E3", "google": "1F9E3", "twitter": "1F9E3" },
            "shortcode": "scarf",
            "category": "people"
        },
        {
            "name": "gloves",
            "unicode": { "apple": "1F9E4", "google": "1F9E4", "twitter": "1F9E4" },
            "shortcode": "gloves",
            "category": "people"
        },
        {
            "name": "coat",
            "unicode": { "apple": "1F9E5", "google": "1F9E5", "twitter": "1F9E5" },
            "shortcode": "coat",
            "category": "people"
        },
        {
            "name": "socks",
            "unicode": { "apple": "1F9E6", "google": "1F9E6", "twitter": "1F9E6" },
            "shortcode": "socks",
            "category": "people"
        },
        {
            "name": "dress",
            "unicode": { "apple": "1F457", "google": "1F457", "twitter": "1F457" },
            "shortcode": "dress",
            "category": "people"
        },
        {
            "name": "kimono",
            "unicode": { "apple": "1F458", "google": "1F458", "twitter": "1F458" },
            "shortcode": "kimono",
            "category": "people"
        },
        {
            "name": "bikini",
            "unicode": { "apple": "1F459", "google": "1F459", "twitter": "1F459" },
            "shortcode": "bikini",
            "category": "people"
        },
        {
            "name": "womans_clothes",
            "unicode": { "apple": "1F45A", "google": "1F45A", "twitter": "1F45A" },
            "shortcode": "womans_clothes",
            "category": "people"
        },
        {
            "name": "purse",
            "unicode": { "apple": "1F45B", "google": "1F45B", "twitter": "1F45B" },
            "shortcode": "purse",
            "category": "people"
        },
        {
            "name": "handbag",
            "unicode": { "apple": "1F45C", "google": "1F45C", "twitter": "1F45C" },
            "shortcode": "handbag",
            "category": "people"
        },
        {
            "name": "pouch",
            "unicode": { "apple": "1F45D", "google": "1F45D", "twitter": "1F45D" },
            "shortcode": "pouch",
            "category": "people"
        },
        {
            "name": "shopping_bags",
            "unicode": { "apple": "1F6CD-FE0F", "google": "1F6CD-FE0F", "twitter": "1F6CD-FE0F" },
            "shortcode": "shopping_bags",
            "category": "people"
        },
        {
            "name": "school_satchel",
            "unicode": { "apple": "1F392", "google": "1F392", "twitter": "1F392" },
            "shortcode": "school_satchel",
            "category": "people"
        },
        {
            "name": "mans_shoe",
            "unicode": { "apple": "1F45E", "google": "1F45E", "twitter": "1F45E" },
            "shortcode": "mans_shoe",
            "category": "people"
        },
        {
            "name": "athletic_shoe",
            "unicode": { "apple": "1F45F", "google": "1F45F", "twitter": "1F45F" },
            "shortcode": "athletic_shoe",
            "category": "people"
        },
        {
            "name": "high_heel",
            "unicode": { "apple": "1F460", "google": "1F460", "twitter": "1F460" },
            "shortcode": "high_heel",
            "category": "people"
        },
        {
            "name": "sandal",
            "unicode": { "apple": "1F461", "google": "1F461", "twitter": "1F461" },
            "shortcode": "sandal",
            "category": "people"
        },
        {
            "name": "boot",
            "unicode": { "apple": "1F462", "google": "1F462", "twitter": "1F462" },
            "shortcode": "boot",
            "category": "people"
        },
        {
            "name": "crown",
            "unicode": { "apple": "1F451", "google": "1F451", "twitter": "1F451" },
            "shortcode": "crown",
            "category": "people"
        },
        {
            "name": "womans_hat",
            "unicode": { "apple": "1F452", "google": "1F452", "twitter": "1F452" },
            "shortcode": "womans_hat",
            "category": "people"
        },
        {
            "name": "tophat",
            "unicode": { "apple": "1F3A9", "google": "1F3A9", "twitter": "1F3A9" },
            "shortcode": "tophat",
            "category": "people"
        },
        {
            "name": "mortar_board",
            "unicode": { "apple": "1F393", "google": "1F393", "twitter": "1F393" },
            "shortcode": "mortar_board",
            "category": "people"
        },
        {
            "name": "billed_cap",
            "unicode": { "apple": "1F9E2", "google": "1F9E2", "twitter": "1F9E2" },
            "shortcode": "billed_cap",
            "category": "people"
        },
        {
            "name": "helmet_with_white_cross",
            "unicode": { "apple": "26D1-FE0F", "google": "26D1-FE0F", "twitter": "26D1-FE0F" },
            "shortcode": "helmet_with_white_cross",
            "category": "people"
        },
        {
            "name": "prayer_beads",
            "unicode": { "apple": "1F4FF", "google": "1F4FF", "twitter": "1F4FF" },
            "shortcode": "prayer_beads",
            "category": "people"
        },
        {
            "name": "lipstick",
            "unicode": { "apple": "1F484", "google": "1F484", "twitter": "1F484" },
            "shortcode": "lipstick",
            "category": "people"
        },
        {
            "name": "ring",
            "unicode": { "apple": "1F48D", "google": "1F48D", "twitter": "1F48D" },
            "shortcode": "ring",
            "category": "people"
        },
        {
            "name": "gem",
            "unicode": { "apple": "1F48E", "google": "1F48E", "twitter": "1F48E" },
            "shortcode": "gem",
            "category": "people"
        },
        {
            "name": "monkey_face",
            "unicode": { "apple": "1F435", "google": "1F435", "twitter": "1F435" },
            "shortcode": "monkey_face",
            "category": "nature"
        },
        {
            "name": "monkey",
            "unicode": { "apple": "1F412", "google": "1F412", "twitter": "1F412" },
            "shortcode": "monkey",
            "category": "nature"
        },
        {
            "name": "gorilla",
            "unicode": { "apple": "1F98D", "google": "1F98D", "twitter": "1F98D" },
            "shortcode": "gorilla",
            "category": "nature"
        },
        {
            "name": "dog",
            "unicode": { "apple": "1F436", "google": "1F436", "twitter": "1F436" },
            "shortcode": "dog",
            "category": "nature"
        },
        {
            "name": "dog2",
            "unicode": { "apple": "1F415", "google": "1F415", "twitter": "1F415" },
            "shortcode": "dog2",
            "category": "nature"
        },
        {
            "name": "poodle",
            "unicode": { "apple": "1F429", "google": "1F429", "twitter": "1F429" },
            "shortcode": "poodle",
            "category": "nature"
        },
        {
            "name": "wolf",
            "unicode": { "apple": "1F43A", "google": "1F43A", "twitter": "1F43A" },
            "shortcode": "wolf",
            "category": "nature"
        },
        {
            "name": "fox_face",
            "unicode": { "apple": "1F98A", "google": "1F98A", "twitter": "1F98A" },
            "shortcode": "fox_face",
            "category": "nature"
        },
        {
            "name": "cat",
            "unicode": { "apple": "1F431", "google": "1F431", "twitter": "1F431" },
            "shortcode": "cat",
            "category": "nature"
        },
        {
            "name": "cat2",
            "unicode": { "apple": "1F408", "google": "1F408", "twitter": "1F408" },
            "shortcode": "cat2",
            "category": "nature"
        },
        {
            "name": "lion_face",
            "unicode": { "apple": "1F981", "google": "1F981", "twitter": "1F981" },
            "shortcode": "lion_face",
            "category": "nature"
        },
        {
            "name": "tiger",
            "unicode": { "apple": "1F42F", "google": "1F42F", "twitter": "1F42F" },
            "shortcode": "tiger",
            "category": "nature"
        },
        {
            "name": "tiger2",
            "unicode": { "apple": "1F405", "google": "1F405", "twitter": "1F405" },
            "shortcode": "tiger2",
            "category": "nature"
        },
        {
            "name": "leopard",
            "unicode": { "apple": "1F406", "google": "1F406", "twitter": "1F406" },
            "shortcode": "leopard",
            "category": "nature"
        },
        {
            "name": "horse",
            "unicode": { "apple": "1F434", "google": "1F434", "twitter": "1F434" },
            "shortcode": "horse",
            "category": "nature"
        },
        {
            "name": "racehorse",
            "unicode": { "apple": "1F40E", "google": "1F40E", "twitter": "1F40E" },
            "shortcode": "racehorse",
            "category": "nature"
        },
        {
            "name": "unicorn_face",
            "unicode": { "apple": "1F984", "google": "1F984", "twitter": "1F984" },
            "shortcode": "unicorn_face",
            "category": "nature"
        },
        {
            "name": "zebra_face",
            "unicode": { "apple": "1F993", "google": "1F993", "twitter": "1F993" },
            "shortcode": "zebra_face",
            "category": "nature"
        },
        {
            "name": "deer",
            "unicode": { "apple": "1F98C", "google": "1F98C", "twitter": "1F98C" },
            "shortcode": "deer",
            "category": "nature"
        },
        {
            "name": "cow",
            "unicode": { "apple": "1F42E", "google": "1F42E", "twitter": "1F42E" },
            "shortcode": "cow",
            "category": "nature"
        },
        {
            "name": "ox",
            "unicode": { "apple": "1F402", "google": "1F402", "twitter": "1F402" },
            "shortcode": "ox",
            "category": "nature"
        },
        {
            "name": "water_buffalo",
            "unicode": { "apple": "1F403", "google": "1F403", "twitter": "1F403" },
            "shortcode": "water_buffalo",
            "category": "nature"
        },
        {
            "name": "cow2",
            "unicode": { "apple": "1F404", "google": "1F404", "twitter": "1F404" },
            "shortcode": "cow2",
            "category": "nature"
        },
        {
            "name": "pig",
            "unicode": { "apple": "1F437", "google": "1F437", "twitter": "1F437" },
            "shortcode": "pig",
            "category": "nature"
        },
        {
            "name": "pig2",
            "unicode": { "apple": "1F416", "google": "1F416", "twitter": "1F416" },
            "shortcode": "pig2",
            "category": "nature"
        },
        {
            "name": "boar",
            "unicode": { "apple": "1F417", "google": "1F417", "twitter": "1F417" },
            "shortcode": "boar",
            "category": "nature"
        },
        {
            "name": "pig_nose",
            "unicode": { "apple": "1F43D", "google": "1F43D", "twitter": "1F43D" },
            "shortcode": "pig_nose",
            "category": "nature"
        },
        {
            "name": "ram",
            "unicode": { "apple": "1F40F", "google": "1F40F", "twitter": "1F40F" },
            "shortcode": "ram",
            "category": "nature"
        },
        {
            "name": "sheep",
            "unicode": { "apple": "1F411", "google": "1F411", "twitter": "1F411" },
            "shortcode": "sheep",
            "category": "nature"
        },
        {
            "name": "goat",
            "unicode": { "apple": "1F410", "google": "1F410", "twitter": "1F410" },
            "shortcode": "goat",
            "category": "nature"
        },
        {
            "name": "dromedary_camel",
            "unicode": { "apple": "1F42A", "google": "1F42A", "twitter": "1F42A" },
            "shortcode": "dromedary_camel",
            "category": "nature"
        },
        {
            "name": "camel",
            "unicode": { "apple": "1F42B", "google": "1F42B", "twitter": "1F42B" },
            "shortcode": "camel",
            "category": "nature"
        },
        {
            "name": "giraffe_face",
            "unicode": { "apple": "1F992", "google": "1F992", "twitter": "1F992" },
            "shortcode": "giraffe_face",
            "category": "nature"
        },
        {
            "name": "elephant",
            "unicode": { "apple": "1F418", "google": "1F418", "twitter": "1F418" },
            "shortcode": "elephant",
            "category": "nature"
        },
        {
            "name": "rhinoceros",
            "unicode": { "apple": "1F98F", "google": "1F98F", "twitter": "1F98F" },
            "shortcode": "rhinoceros",
            "category": "nature"
        },
        {
            "name": "mouse",
            "unicode": { "apple": "1F42D", "google": "1F42D", "twitter": "1F42D" },
            "shortcode": "mouse",
            "category": "nature"
        },
        {
            "name": "mouse2",
            "unicode": { "apple": "1F401", "google": "1F401", "twitter": "1F401" },
            "shortcode": "mouse2",
            "category": "nature"
        },
        {
            "name": "rat",
            "unicode": { "apple": "1F400", "google": "1F400", "twitter": "1F400" },
            "shortcode": "rat",
            "category": "nature"
        },
        {
            "name": "hamster",
            "unicode": { "apple": "1F439", "google": "1F439", "twitter": "1F439" },
            "shortcode": "hamster",
            "category": "nature"
        },
        {
            "name": "rabbit",
            "unicode": { "apple": "1F430", "google": "1F430", "twitter": "1F430" },
            "shortcode": "rabbit",
            "category": "nature"
        },
        {
            "name": "rabbit2",
            "unicode": { "apple": "1F407", "google": "1F407", "twitter": "1F407" },
            "shortcode": "rabbit2",
            "category": "nature"
        },
        {
            "name": "chipmunk",
            "unicode": { "apple": "1F43F-FE0F", "google": "1F43F-FE0F", "twitter": "1F43F-FE0F" },
            "shortcode": "chipmunk",
            "category": "nature"
        },
        {
            "name": "hedgehog",
            "unicode": { "apple": "1F994", "google": "1F994", "twitter": "1F994" },
            "shortcode": "hedgehog",
            "category": "nature"
        },
        {
            "name": "bat",
            "unicode": { "apple": "1F987", "google": "1F987", "twitter": "1F987" },
            "shortcode": "bat",
            "category": "nature"
        },
        {
            "name": "bear",
            "unicode": { "apple": "1F43B", "google": "1F43B", "twitter": "1F43B" },
            "shortcode": "bear",
            "category": "nature"
        },
        {
            "name": "koala",
            "unicode": { "apple": "1F428", "google": "1F428", "twitter": "1F428" },
            "shortcode": "koala",
            "category": "nature"
        },
        {
            "name": "panda_face",
            "unicode": { "apple": "1F43C", "google": "1F43C", "twitter": "1F43C" },
            "shortcode": "panda_face",
            "category": "nature"
        },
        {
            "name": "feet",
            "unicode": { "apple": "1F43E", "google": "1F43E", "twitter": "1F43E" },
            "shortcode": "feet",
            "category": "nature"
        },
        {
            "name": "turkey",
            "unicode": { "apple": "1F983", "google": "1F983", "twitter": "1F983" },
            "shortcode": "turkey",
            "category": "nature"
        },
        {
            "name": "chicken",
            "unicode": { "apple": "1F414", "google": "1F414", "twitter": "1F414" },
            "shortcode": "chicken",
            "category": "nature"
        },
        {
            "name": "rooster",
            "unicode": { "apple": "1F413", "google": "1F413", "twitter": "1F413" },
            "shortcode": "rooster",
            "category": "nature"
        },
        {
            "name": "hatching_chick",
            "unicode": { "apple": "1F423", "google": "1F423", "twitter": "1F423" },
            "shortcode": "hatching_chick",
            "category": "nature"
        },
        {
            "name": "baby_chick",
            "unicode": { "apple": "1F424", "google": "1F424", "twitter": "1F424" },
            "shortcode": "baby_chick",
            "category": "nature"
        },
        {
            "name": "hatched_chick",
            "unicode": { "apple": "1F425", "google": "1F425", "twitter": "1F425" },
            "shortcode": "hatched_chick",
            "category": "nature"
        },
        {
            "name": "bird",
            "unicode": { "apple": "1F426", "google": "1F426", "twitter": "1F426" },
            "shortcode": "bird",
            "category": "nature"
        },
        {
            "name": "penguin",
            "unicode": { "apple": "1F427", "google": "1F427", "twitter": "1F427" },
            "shortcode": "penguin",
            "category": "nature"
        },
        {
            "name": "dove_of_peace",
            "unicode": { "apple": "1F54A-FE0F", "google": "1F54A-FE0F", "twitter": "1F54A-FE0F" },
            "shortcode": "dove_of_peace",
            "category": "nature"
        },
        {
            "name": "eagle",
            "unicode": { "apple": "1F985", "google": "1F985", "twitter": "1F985" },
            "shortcode": "eagle",
            "category": "nature"
        },
        {
            "name": "duck",
            "unicode": { "apple": "1F986", "google": "1F986", "twitter": "1F986" },
            "shortcode": "duck",
            "category": "nature"
        },
        {
            "name": "owl",
            "unicode": { "apple": "1F989", "google": "1F989", "twitter": "1F989" },
            "shortcode": "owl",
            "category": "nature"
        },
        {
            "name": "frog",
            "unicode": { "apple": "1F438", "google": "1F438", "twitter": "1F438" },
            "shortcode": "frog",
            "category": "nature"
        },
        {
            "name": "crocodile",
            "unicode": { "apple": "1F40A", "google": "1F40A", "twitter": "1F40A" },
            "shortcode": "crocodile",
            "category": "nature"
        },
        {
            "name": "turtle",
            "unicode": { "apple": "1F422", "google": "1F422", "twitter": "1F422" },
            "shortcode": "turtle",
            "category": "nature"
        },
        {
            "name": "lizard",
            "unicode": { "apple": "1F98E", "google": "1F98E", "twitter": "1F98E" },
            "shortcode": "lizard",
            "category": "nature"
        },
        {
            "name": "snake",
            "unicode": { "apple": "1F40D", "google": "1F40D", "twitter": "1F40D" },
            "shortcode": "snake",
            "category": "nature"
        },
        {
            "name": "dragon_face",
            "unicode": { "apple": "1F432", "google": "1F432", "twitter": "1F432" },
            "shortcode": "dragon_face",
            "category": "nature"
        },
        {
            "name": "dragon",
            "unicode": { "apple": "1F409", "google": "1F409", "twitter": "1F409" },
            "shortcode": "dragon",
            "category": "nature"
        },
        {
            "name": "sauropod",
            "unicode": { "apple": "1F995", "google": "1F995", "twitter": "1F995" },
            "shortcode": "sauropod",
            "category": "nature"
        },
        {
            "name": "t-rex",
            "unicode": { "apple": "1F996", "google": "1F996", "twitter": "1F996" },
            "shortcode": "t-rex",
            "category": "nature"
        },
        {
            "name": "whale",
            "unicode": { "apple": "1F433", "google": "1F433", "twitter": "1F433" },
            "shortcode": "whale",
            "category": "nature"
        },
        {
            "name": "whale2",
            "unicode": { "apple": "1F40B", "google": "1F40B", "twitter": "1F40B" },
            "shortcode": "whale2",
            "category": "nature"
        },
        {
            "name": "dolphin",
            "unicode": { "apple": "1F42C", "google": "1F42C", "twitter": "1F42C" },
            "shortcode": "dolphin",
            "category": "nature"
        },
        {
            "name": "fish",
            "unicode": { "apple": "1F41F", "google": "1F41F", "twitter": "1F41F" },
            "shortcode": "fish",
            "category": "nature"
        },
        {
            "name": "tropical_fish",
            "unicode": { "apple": "1F420", "google": "1F420", "twitter": "1F420" },
            "shortcode": "tropical_fish",
            "category": "nature"
        },
        {
            "name": "blowfish",
            "unicode": { "apple": "1F421", "google": "1F421", "twitter": "1F421" },
            "shortcode": "blowfish",
            "category": "nature"
        },
        {
            "name": "shark",
            "unicode": { "apple": "1F988", "google": "1F988", "twitter": "1F988" },
            "shortcode": "shark",
            "category": "nature"
        },
        {
            "name": "octopus",
            "unicode": { "apple": "1F419", "google": "1F419", "twitter": "1F419" },
            "shortcode": "octopus",
            "category": "nature"
        },
        {
            "name": "shell",
            "unicode": { "apple": "1F41A", "google": "1F41A", "twitter": "1F41A" },
            "shortcode": "shell",
            "category": "nature"
        },
        {
            "name": "crab",
            "unicode": { "apple": "1F980", "google": "1F980", "twitter": "1F980" },
            "shortcode": "crab",
            "category": "nature"
        },
        {
            "name": "shrimp",
            "unicode": { "apple": "1F990", "google": "1F990", "twitter": "1F990" },
            "shortcode": "shrimp",
            "category": "nature"
        },
        {
            "name": "squid",
            "unicode": { "apple": "1F991", "google": "1F991", "twitter": "1F991" },
            "shortcode": "squid",
            "category": "nature"
        },
        {
            "name": "snail",
            "unicode": { "apple": "1F40C", "google": "1F40C", "twitter": "1F40C" },
            "shortcode": "snail",
            "category": "nature"
        },
        {
            "name": "butterfly",
            "unicode": { "apple": "1F98B", "google": "1F98B", "twitter": "1F98B" },
            "shortcode": "butterfly",
            "category": "nature"
        },
        {
            "name": "bug",
            "unicode": { "apple": "1F41B", "google": "1F41B", "twitter": "1F41B" },
            "shortcode": "bug",
            "category": "nature"
        },
        {
            "name": "ant",
            "unicode": { "apple": "1F41C", "google": "1F41C", "twitter": "1F41C" },
            "shortcode": "ant",
            "category": "nature"
        },
        {
            "name": "bee",
            "unicode": { "apple": "1F41D", "google": "1F41D", "twitter": "1F41D" },
            "shortcode": "bee",
            "category": "nature"
        },
        {
            "name": "beetle",
            "unicode": { "apple": "1F41E", "google": "1F41E", "twitter": "1F41E" },
            "shortcode": "beetle",
            "category": "nature"
        },
        {
            "name": "cricket",
            "unicode": { "apple": "1F997", "google": "1F997", "twitter": "1F997" },
            "shortcode": "cricket",
            "category": "nature"
        },
        {
            "name": "spider",
            "unicode": { "apple": "1F577-FE0F", "google": "1F577-FE0F", "twitter": "1F577-FE0F" },
            "shortcode": "spider",
            "category": "nature"
        },
        {
            "name": "spider_web",
            "unicode": { "apple": "1F578-FE0F", "google": "1F578-FE0F", "twitter": "1F578-FE0F" },
            "shortcode": "spider_web",
            "category": "nature"
        },
        {
            "name": "scorpion",
            "unicode": { "apple": "1F982", "google": "1F982", "twitter": "1F982" },
            "shortcode": "scorpion",
            "category": "nature"
        },
        {
            "name": "bouquet",
            "unicode": { "apple": "1F490", "google": "1F490", "twitter": "1F490" },
            "shortcode": "bouquet",
            "category": "nature"
        },
        {
            "name": "cherry_blossom",
            "unicode": { "apple": "1F338", "google": "1F338", "twitter": "1F338" },
            "shortcode": "cherry_blossom",
            "category": "nature"
        },
        {
            "name": "white_flower",
            "unicode": { "apple": "1F4AE", "google": "1F4AE", "twitter": "1F4AE" },
            "shortcode": "white_flower",
            "category": "nature"
        },
        {
            "name": "rosette",
            "unicode": { "apple": "1F3F5-FE0F", "google": "1F3F5-FE0F", "twitter": "1F3F5-FE0F" },
            "shortcode": "rosette",
            "category": "nature"
        },
        {
            "name": "rose",
            "unicode": { "apple": "1F339", "google": "1F339", "twitter": "1F339" },
            "shortcode": "rose",
            "category": "nature"
        },
        {
            "name": "wilted_flower",
            "unicode": { "apple": "1F940", "google": "1F940", "twitter": "1F940" },
            "shortcode": "wilted_flower",
            "category": "nature"
        },
        {
            "name": "hibiscus",
            "unicode": { "apple": "1F33A", "google": "1F33A", "twitter": "1F33A" },
            "shortcode": "hibiscus",
            "category": "nature"
        },
        {
            "name": "sunflower",
            "unicode": { "apple": "1F33B", "google": "1F33B", "twitter": "1F33B" },
            "shortcode": "sunflower",
            "category": "nature"
        },
        {
            "name": "blossom",
            "unicode": { "apple": "1F33C", "google": "1F33C", "twitter": "1F33C" },
            "shortcode": "blossom",
            "category": "nature"
        },
        {
            "name": "tulip",
            "unicode": { "apple": "1F337", "google": "1F337", "twitter": "1F337" },
            "shortcode": "tulip",
            "category": "nature"
        },
        {
            "name": "seedling",
            "unicode": { "apple": "1F331", "google": "1F331", "twitter": "1F331" },
            "shortcode": "seedling",
            "category": "nature"
        },
        {
            "name": "evergreen_tree",
            "unicode": { "apple": "1F332", "google": "1F332", "twitter": "1F332" },
            "shortcode": "evergreen_tree",
            "category": "nature"
        },
        {
            "name": "deciduous_tree",
            "unicode": { "apple": "1F333", "google": "1F333", "twitter": "1F333" },
            "shortcode": "deciduous_tree",
            "category": "nature"
        },
        {
            "name": "palm_tree",
            "unicode": { "apple": "1F334", "google": "1F334", "twitter": "1F334" },
            "shortcode": "palm_tree",
            "category": "nature"
        },
        {
            "name": "cactus",
            "unicode": { "apple": "1F335", "google": "1F335", "twitter": "1F335" },
            "shortcode": "cactus",
            "category": "nature"
        },
        {
            "name": "ear_of_rice",
            "unicode": { "apple": "1F33E", "google": "1F33E", "twitter": "1F33E" },
            "shortcode": "ear_of_rice",
            "category": "nature"
        },
        {
            "name": "herb",
            "unicode": { "apple": "1F33F", "google": "1F33F", "twitter": "1F33F" },
            "shortcode": "herb",
            "category": "nature"
        },
        {
            "name": "shamrock",
            "unicode": { "apple": "2618-FE0F", "google": "2618-FE0F", "twitter": "2618-FE0F" },
            "shortcode": "shamrock",
            "category": "nature"
        },
        {
            "name": "four_leaf_clover",
            "unicode": { "apple": "1F340", "google": "1F340", "twitter": "1F340" },
            "shortcode": "four_leaf_clover",
            "category": "nature"
        },
        {
            "name": "maple_leaf",
            "unicode": { "apple": "1F341", "google": "1F341", "twitter": "1F341" },
            "shortcode": "maple_leaf",
            "category": "nature"
        },
        {
            "name": "fallen_leaf",
            "unicode": { "apple": "1F342", "google": "1F342", "twitter": "1F342" },
            "shortcode": "fallen_leaf",
            "category": "nature"
        },
        {
            "name": "leaves",
            "unicode": { "apple": "1F343", "google": "1F343", "twitter": "1F343" },
            "shortcode": "leaves",
            "category": "nature"
        },
        {
            "name": "grapes",
            "unicode": { "apple": "1F347", "google": "1F347", "twitter": "1F347" },
            "shortcode": "grapes",
            "category": "food"
        },
        {
            "name": "melon",
            "unicode": { "apple": "1F348", "google": "1F348", "twitter": "1F348" },
            "shortcode": "melon",
            "category": "food"
        },
        {
            "name": "watermelon",
            "unicode": { "apple": "1F349", "google": "1F349", "twitter": "1F349" },
            "shortcode": "watermelon",
            "category": "food"
        },
        {
            "name": "tangerine",
            "unicode": { "apple": "1F34A", "google": "1F34A", "twitter": "1F34A" },
            "shortcode": "tangerine",
            "category": "food"
        },
        {
            "name": "lemon",
            "unicode": { "apple": "1F34B", "google": "1F34B", "twitter": "1F34B" },
            "shortcode": "lemon",
            "category": "food"
        },
        {
            "name": "banana",
            "unicode": { "apple": "1F34C", "google": "1F34C", "twitter": "1F34C" },
            "shortcode": "banana",
            "category": "food"
        },
        {
            "name": "pineapple",
            "unicode": { "apple": "1F34D", "google": "1F34D", "twitter": "1F34D" },
            "shortcode": "pineapple",
            "category": "food"
        },
        {
            "name": "apple",
            "unicode": { "apple": "1F34E", "google": "1F34E", "twitter": "1F34E" },
            "shortcode": "apple",
            "category": "food"
        },
        {
            "name": "green_apple",
            "unicode": { "apple": "1F34F", "google": "1F34F", "twitter": "1F34F" },
            "shortcode": "green_apple",
            "category": "food"
        },
        {
            "name": "pear",
            "unicode": { "apple": "1F350", "google": "1F350", "twitter": "1F350" },
            "shortcode": "pear",
            "category": "food"
        },
        {
            "name": "peach",
            "unicode": { "apple": "1F351", "google": "1F351", "twitter": "1F351" },
            "shortcode": "peach",
            "category": "food"
        },
        {
            "name": "cherries",
            "unicode": { "apple": "1F352", "google": "1F352", "twitter": "1F352" },
            "shortcode": "cherries",
            "category": "food"
        },
        {
            "name": "strawberry",
            "unicode": { "apple": "1F353", "google": "1F353", "twitter": "1F353" },
            "shortcode": "strawberry",
            "category": "food"
        },
        {
            "name": "kiwifruit",
            "unicode": { "apple": "1F95D", "google": "1F95D", "twitter": "1F95D" },
            "shortcode": "kiwifruit",
            "category": "food"
        },
        {
            "name": "tomato",
            "unicode": { "apple": "1F345", "google": "1F345", "twitter": "1F345" },
            "shortcode": "tomato",
            "category": "food"
        },
        {
            "name": "coconut",
            "unicode": { "apple": "1F965", "google": "1F965", "twitter": "1F965" },
            "shortcode": "coconut",
            "category": "food"
        },
        {
            "name": "avocado",
            "unicode": { "apple": "1F951", "google": "1F951", "twitter": "1F951" },
            "shortcode": "avocado",
            "category": "food"
        },
        {
            "name": "eggplant",
            "unicode": { "apple": "1F346", "google": "1F346", "twitter": "1F346" },
            "shortcode": "eggplant",
            "category": "food"
        },
        {
            "name": "potato",
            "unicode": { "apple": "1F954", "google": "1F954", "twitter": "1F954" },
            "shortcode": "potato",
            "category": "food"
        },
        {
            "name": "carrot",
            "unicode": { "apple": "1F955", "google": "1F955", "twitter": "1F955" },
            "shortcode": "carrot",
            "category": "food"
        },
        {
            "name": "corn",
            "unicode": { "apple": "1F33D", "google": "1F33D", "twitter": "1F33D" },
            "shortcode": "corn",
            "category": "food"
        },
        {
            "name": "hot_pepper",
            "unicode": { "apple": "1F336-FE0F", "google": "1F336-FE0F", "twitter": "1F336-FE0F" },
            "shortcode": "hot_pepper",
            "category": "food"
        },
        {
            "name": "cucumber",
            "unicode": { "apple": "1F952", "google": "1F952", "twitter": "1F952" },
            "shortcode": "cucumber",
            "category": "food"
        },
        {
            "name": "broccoli",
            "unicode": { "apple": "1F966", "google": "1F966", "twitter": "1F966" },
            "shortcode": "broccoli",
            "category": "food"
        },
        {
            "name": "mushroom",
            "unicode": { "apple": "1F344", "google": "1F344", "twitter": "1F344" },
            "shortcode": "mushroom",
            "category": "food"
        },
        {
            "name": "peanuts",
            "unicode": { "apple": "1F95C", "google": "1F95C", "twitter": "1F95C" },
            "shortcode": "peanuts",
            "category": "food"
        },
        {
            "name": "chestnut",
            "unicode": { "apple": "1F330", "google": "1F330", "twitter": "1F330" },
            "shortcode": "chestnut",
            "category": "food"
        },
        {
            "name": "bread",
            "unicode": { "apple": "1F35E", "google": "1F35E", "twitter": "1F35E" },
            "shortcode": "bread",
            "category": "food"
        },
        {
            "name": "croissant",
            "unicode": { "apple": "1F950", "google": "1F950", "twitter": "1F950" },
            "shortcode": "croissant",
            "category": "food"
        },
        {
            "name": "baguette_bread",
            "unicode": { "apple": "1F956", "google": "1F956", "twitter": "1F956" },
            "shortcode": "baguette_bread",
            "category": "food"
        },
        {
            "name": "pretzel",
            "unicode": { "apple": "1F968", "google": "1F968", "twitter": "1F968" },
            "shortcode": "pretzel",
            "category": "food"
        },
        {
            "name": "pancakes",
            "unicode": { "apple": "1F95E", "google": "1F95E", "twitter": "1F95E" },
            "shortcode": "pancakes",
            "category": "food"
        },
        {
            "name": "cheese_wedge",
            "unicode": { "apple": "1F9C0", "google": "1F9C0", "twitter": "1F9C0" },
            "shortcode": "cheese_wedge",
            "category": "food"
        },
        {
            "name": "meat_on_bone",
            "unicode": { "apple": "1F356", "google": "1F356", "twitter": "1F356" },
            "shortcode": "meat_on_bone",
            "category": "food"
        },
        {
            "name": "poultry_leg",
            "unicode": { "apple": "1F357", "google": "1F357", "twitter": "1F357" },
            "shortcode": "poultry_leg",
            "category": "food"
        },
        {
            "name": "cut_of_meat",
            "unicode": { "apple": "1F969", "google": "1F969", "twitter": "1F969" },
            "shortcode": "cut_of_meat",
            "category": "food"
        },
        {
            "name": "bacon",
            "unicode": { "apple": "1F953", "google": "1F953", "twitter": "1F953" },
            "shortcode": "bacon",
            "category": "food"
        },
        {
            "name": "hamburger",
            "unicode": { "apple": "1F354", "google": "1F354", "twitter": "1F354" },
            "shortcode": "hamburger",
            "category": "food"
        },
        {
            "name": "fries",
            "unicode": { "apple": "1F35F", "google": "1F35F", "twitter": "1F35F" },
            "shortcode": "fries",
            "category": "food"
        },
        {
            "name": "pizza",
            "unicode": { "apple": "1F355", "google": "1F355", "twitter": "1F355" },
            "shortcode": "pizza",
            "category": "food"
        },
        {
            "name": "hotdog",
            "unicode": { "apple": "1F32D", "google": "1F32D", "twitter": "1F32D" },
            "shortcode": "hotdog",
            "category": "food"
        },
        {
            "name": "sandwich",
            "unicode": { "apple": "1F96A", "google": "1F96A", "twitter": "1F96A" },
            "shortcode": "sandwich",
            "category": "food"
        },
        {
            "name": "taco",
            "unicode": { "apple": "1F32E", "google": "1F32E", "twitter": "1F32E" },
            "shortcode": "taco",
            "category": "food"
        },
        {
            "name": "burrito",
            "unicode": { "apple": "1F32F", "google": "1F32F", "twitter": "1F32F" },
            "shortcode": "burrito",
            "category": "food"
        },
        {
            "name": "stuffed_flatbread",
            "unicode": { "apple": "1F959", "google": "1F959", "twitter": "1F959" },
            "shortcode": "stuffed_flatbread",
            "category": "food"
        },
        {
            "name": "egg",
            "unicode": { "apple": "1F95A", "google": "1F95A", "twitter": "1F95A" },
            "shortcode": "egg",
            "category": "food"
        },
        {
            "name": "fried_egg",
            "unicode": { "apple": "1F373", "google": "1F373", "twitter": "1F373" },
            "shortcode": "fried_egg",
            "category": "food"
        },
        {
            "name": "shallow_pan_of_food",
            "unicode": { "apple": "1F958", "google": "1F958", "twitter": "1F958" },
            "shortcode": "shallow_pan_of_food",
            "category": "food"
        },
        {
            "name": "stew",
            "unicode": { "apple": "1F372", "google": "1F372", "twitter": "1F372" },
            "shortcode": "stew",
            "category": "food"
        },
        {
            "name": "bowl_with_spoon",
            "unicode": { "apple": "1F963", "google": "1F963", "twitter": "1F963" },
            "shortcode": "bowl_with_spoon",
            "category": "food"
        },
        {
            "name": "green_salad",
            "unicode": { "apple": "1F957", "google": "1F957", "twitter": "1F957" },
            "shortcode": "green_salad",
            "category": "food"
        },
        {
            "name": "popcorn",
            "unicode": { "apple": "1F37F", "google": "1F37F", "twitter": "1F37F" },
            "shortcode": "popcorn",
            "category": "food"
        },
        {
            "name": "canned_food",
            "unicode": { "apple": "1F96B", "google": "1F96B", "twitter": "1F96B" },
            "shortcode": "canned_food",
            "category": "food"
        },
        {
            "name": "bento",
            "unicode": { "apple": "1F371", "google": "1F371", "twitter": "1F371" },
            "shortcode": "bento",
            "category": "food"
        },
        {
            "name": "rice_cracker",
            "unicode": { "apple": "1F358", "google": "1F358", "twitter": "1F358" },
            "shortcode": "rice_cracker",
            "category": "food"
        },
        {
            "name": "rice_ball",
            "unicode": { "apple": "1F359", "google": "1F359", "twitter": "1F359" },
            "shortcode": "rice_ball",
            "category": "food"
        },
        {
            "name": "rice",
            "unicode": { "apple": "1F35A", "google": "1F35A", "twitter": "1F35A" },
            "shortcode": "rice",
            "category": "food"
        },
        {
            "name": "curry",
            "unicode": { "apple": "1F35B", "google": "1F35B", "twitter": "1F35B" },
            "shortcode": "curry",
            "category": "food"
        },
        {
            "name": "ramen",
            "unicode": { "apple": "1F35C", "google": "1F35C", "twitter": "1F35C" },
            "shortcode": "ramen",
            "category": "food"
        },
        {
            "name": "spaghetti",
            "unicode": { "apple": "1F35D", "google": "1F35D", "twitter": "1F35D" },
            "shortcode": "spaghetti",
            "category": "food"
        },
        {
            "name": "sweet_potato",
            "unicode": { "apple": "1F360", "google": "1F360", "twitter": "1F360" },
            "shortcode": "sweet_potato",
            "category": "food"
        },
        {
            "name": "oden",
            "unicode": { "apple": "1F362", "google": "1F362", "twitter": "1F362" },
            "shortcode": "oden",
            "category": "food"
        },
        {
            "name": "sushi",
            "unicode": { "apple": "1F363", "google": "1F363", "twitter": "1F363" },
            "shortcode": "sushi",
            "category": "food"
        },
        {
            "name": "fried_shrimp",
            "unicode": { "apple": "1F364", "google": "1F364", "twitter": "1F364" },
            "shortcode": "fried_shrimp",
            "category": "food"
        },
        {
            "name": "fish_cake",
            "unicode": { "apple": "1F365", "google": "1F365", "twitter": "1F365" },
            "shortcode": "fish_cake",
            "category": "food"
        },
        {
            "name": "dango",
            "unicode": { "apple": "1F361", "google": "1F361", "twitter": "1F361" },
            "shortcode": "dango",
            "category": "food"
        },
        {
            "name": "dumpling",
            "unicode": { "apple": "1F95F", "google": "1F95F", "twitter": "1F95F" },
            "shortcode": "dumpling",
            "category": "food"
        },
        {
            "name": "fortune_cookie",
            "unicode": { "apple": "1F960", "google": "1F960", "twitter": "1F960" },
            "shortcode": "fortune_cookie",
            "category": "food"
        },
        {
            "name": "takeout_box",
            "unicode": { "apple": "1F961", "google": "1F961", "twitter": "1F961" },
            "shortcode": "takeout_box",
            "category": "food"
        },
        {
            "name": "icecream",
            "unicode": { "apple": "1F366", "google": "1F366", "twitter": "1F366" },
            "shortcode": "icecream",
            "category": "food"
        },
        {
            "name": "shaved_ice",
            "unicode": { "apple": "1F367", "google": "1F367", "twitter": "1F367" },
            "shortcode": "shaved_ice",
            "category": "food"
        },
        {
            "name": "ice_cream",
            "unicode": { "apple": "1F368", "google": "1F368", "twitter": "1F368" },
            "shortcode": "ice_cream",
            "category": "food"
        },
        {
            "name": "doughnut",
            "unicode": { "apple": "1F369", "google": "1F369", "twitter": "1F369" },
            "shortcode": "doughnut",
            "category": "food"
        },
        {
            "name": "cookie",
            "unicode": { "apple": "1F36A", "google": "1F36A", "twitter": "1F36A" },
            "shortcode": "cookie",
            "category": "food"
        },
        {
            "name": "birthday",
            "unicode": { "apple": "1F382", "google": "1F382", "twitter": "1F382" },
            "shortcode": "birthday",
            "category": "food"
        },
        {
            "name": "cake",
            "unicode": { "apple": "1F370", "google": "1F370", "twitter": "1F370" },
            "shortcode": "cake",
            "category": "food"
        },
        {
            "name": "pie",
            "unicode": { "apple": "1F967", "google": "1F967", "twitter": "1F967" },
            "shortcode": "pie",
            "category": "food"
        },
        {
            "name": "chocolate_bar",
            "unicode": { "apple": "1F36B", "google": "1F36B", "twitter": "1F36B" },
            "shortcode": "chocolate_bar",
            "category": "food"
        },
        {
            "name": "candy",
            "unicode": { "apple": "1F36C", "google": "1F36C", "twitter": "1F36C" },
            "shortcode": "candy",
            "category": "food"
        },
        {
            "name": "lollipop",
            "unicode": { "apple": "1F36D", "google": "1F36D", "twitter": "1F36D" },
            "shortcode": "lollipop",
            "category": "food"
        },
        {
            "name": "custard",
            "unicode": { "apple": "1F36E", "google": "1F36E", "twitter": "1F36E" },
            "shortcode": "custard",
            "category": "food"
        },
        {
            "name": "honey_pot",
            "unicode": { "apple": "1F36F", "google": "1F36F", "twitter": "1F36F" },
            "shortcode": "honey_pot",
            "category": "food"
        },
        {
            "name": "baby_bottle",
            "unicode": { "apple": "1F37C", "google": "1F37C", "twitter": "1F37C" },
            "shortcode": "baby_bottle",
            "category": "food"
        },
        {
            "name": "glass_of_milk",
            "unicode": { "apple": "1F95B", "google": "1F95B", "twitter": "1F95B" },
            "shortcode": "glass_of_milk",
            "category": "food"
        },
        {
            "name": "coffee",
            "unicode": { "apple": "2615", "google": "2615", "twitter": "2615" },
            "shortcode": "coffee",
            "category": "food"
        },
        {
            "name": "tea",
            "unicode": { "apple": "1F375", "google": "1F375", "twitter": "1F375" },
            "shortcode": "tea",
            "category": "food"
        },
        {
            "name": "sake",
            "unicode": { "apple": "1F376", "google": "1F376", "twitter": "1F376" },
            "shortcode": "sake",
            "category": "food"
        },
        {
            "name": "champagne",
            "unicode": { "apple": "1F37E", "google": "1F37E", "twitter": "1F37E" },
            "shortcode": "champagne",
            "category": "food"
        },
        {
            "name": "wine_glass",
            "unicode": { "apple": "1F377", "google": "1F377", "twitter": "1F377" },
            "shortcode": "wine_glass",
            "category": "food"
        },
        {
            "name": "cocktail",
            "unicode": { "apple": "1F378", "google": "1F378", "twitter": "1F378" },
            "shortcode": "cocktail",
            "category": "food"
        },
        {
            "name": "tropical_drink",
            "unicode": { "apple": "1F379", "google": "1F379", "twitter": "1F379" },
            "shortcode": "tropical_drink",
            "category": "food"
        },
        {
            "name": "beer",
            "unicode": { "apple": "1F37A", "google": "1F37A", "twitter": "1F37A" },
            "shortcode": "beer",
            "category": "food"
        },
        {
            "name": "beers",
            "unicode": { "apple": "1F37B", "google": "1F37B", "twitter": "1F37B" },
            "shortcode": "beers",
            "category": "food"
        },
        {
            "name": "clinking_glasses",
            "unicode": { "apple": "1F942", "google": "1F942", "twitter": "1F942" },
            "shortcode": "clinking_glasses",
            "category": "food"
        },
        {
            "name": "tumbler_glass",
            "unicode": { "apple": "1F943", "google": "1F943", "twitter": "1F943" },
            "shortcode": "tumbler_glass",
            "category": "food"
        },
        {
            "name": "cup_with_straw",
            "unicode": { "apple": "1F964", "google": "1F964", "twitter": "1F964" },
            "shortcode": "cup_with_straw",
            "category": "food"
        },
        {
            "name": "chopsticks",
            "unicode": { "apple": "1F962", "google": "1F962", "twitter": "1F962" },
            "shortcode": "chopsticks",
            "category": "food"
        },
        {
            "name": "knife_fork_plate",
            "unicode": { "apple": "1F37D-FE0F", "google": "1F37D-FE0F", "twitter": "1F37D-FE0F" },
            "shortcode": "knife_fork_plate",
            "category": "food"
        },
        {
            "name": "fork_and_knife",
            "unicode": { "apple": "1F374", "google": "1F374", "twitter": "1F374" },
            "shortcode": "fork_and_knife",
            "category": "food"
        },
        {
            "name": "spoon",
            "unicode": { "apple": "1F944", "google": "1F944", "twitter": "1F944" },
            "shortcode": "spoon",
            "category": "food"
        },
        {
            "name": "hocho",
            "unicode": { "apple": "1F52A", "google": "1F52A", "twitter": "1F52A" },
            "shortcode": "hocho",
            "category": "food"
        },
        {
            "name": "amphora",
            "unicode": { "apple": "1F3FA", "google": "1F3FA", "twitter": "1F3FA" },
            "shortcode": "amphora",
            "category": "food"
        },
        {
            "name": "jack_o_lantern",
            "unicode": { "apple": "1F383", "google": "1F383", "twitter": "1F383" },
            "shortcode": "jack_o_lantern",
            "category": "activity"
        },
        {
            "name": "christmas_tree",
            "unicode": { "apple": "1F384", "google": "1F384", "twitter": "1F384" },
            "shortcode": "christmas_tree",
            "category": "activity"
        },
        {
            "name": "fireworks",
            "unicode": { "apple": "1F386", "google": "1F386", "twitter": "1F386" },
            "shortcode": "fireworks",
            "category": "activity"
        },
        {
            "name": "sparkler",
            "unicode": { "apple": "1F387", "google": "1F387", "twitter": "1F387" },
            "shortcode": "sparkler",
            "category": "activity"
        },
        {
            "name": "sparkles",
            "unicode": { "apple": "2728", "google": "2728", "twitter": "2728" },
            "shortcode": "sparkles",
            "category": "activity"
        },
        {
            "name": "balloon",
            "unicode": { "apple": "1F388", "google": "1F388", "twitter": "1F388" },
            "shortcode": "balloon",
            "category": "activity"
        },
        {
            "name": "tada",
            "unicode": { "apple": "1F389", "google": "1F389", "twitter": "1F389" },
            "shortcode": "tada",
            "category": "activity"
        },
        {
            "name": "confetti_ball",
            "unicode": { "apple": "1F38A", "google": "1F38A", "twitter": "1F38A" },
            "shortcode": "confetti_ball",
            "category": "activity"
        },
        {
            "name": "tanabata_tree",
            "unicode": { "apple": "1F38B", "google": "1F38B", "twitter": "1F38B" },
            "shortcode": "tanabata_tree",
            "category": "activity"
        },
        {
            "name": "bamboo",
            "unicode": { "apple": "1F38D", "google": "1F38D", "twitter": "1F38D" },
            "shortcode": "bamboo",
            "category": "activity"
        },
        {
            "name": "dolls",
            "unicode": { "apple": "1F38E", "google": "1F38E", "twitter": "1F38E" },
            "shortcode": "dolls",
            "category": "activity"
        },
        {
            "name": "flags",
            "unicode": { "apple": "1F38F", "google": "1F38F", "twitter": "1F38F" },
            "shortcode": "flags",
            "category": "activity"
        },
        {
            "name": "wind_chime",
            "unicode": { "apple": "1F390", "google": "1F390", "twitter": "1F390" },
            "shortcode": "wind_chime",
            "category": "activity"
        },
        {
            "name": "rice_scene",
            "unicode": { "apple": "1F391", "google": "1F391", "twitter": "1F391" },
            "shortcode": "rice_scene",
            "category": "activity"
        },
        {
            "name": "ribbon",
            "unicode": { "apple": "1F380", "google": "1F380", "twitter": "1F380" },
            "shortcode": "ribbon",
            "category": "activity"
        },
        {
            "name": "gift",
            "unicode": { "apple": "1F381", "google": "1F381", "twitter": "1F381" },
            "shortcode": "gift",
            "category": "activity"
        },
        {
            "name": "reminder_ribbon",
            "unicode": { "apple": "1F397-FE0F", "google": "1F397-FE0F", "twitter": "1F397-FE0F" },
            "shortcode": "reminder_ribbon",
            "category": "activity"
        },
        {
            "name": "admission_tickets",
            "unicode": { "apple": "1F39F-FE0F", "google": "1F39F-FE0F", "twitter": "1F39F-FE0F" },
            "shortcode": "admission_tickets",
            "category": "activity"
        },
        {
            "name": "ticket",
            "unicode": { "apple": "1F3AB", "google": "1F3AB", "twitter": "1F3AB" },
            "shortcode": "ticket",
            "category": "activity"
        },
        {
            "name": "medal",
            "unicode": { "apple": "1F396-FE0F", "google": "1F396-FE0F", "twitter": "1F396-FE0F" },
            "shortcode": "medal",
            "category": "activity"
        },
        {
            "name": "trophy",
            "unicode": { "apple": "1F3C6", "google": "1F3C6", "twitter": "1F3C6" },
            "shortcode": "trophy",
            "category": "activity"
        },
        {
            "name": "sports_medal",
            "unicode": { "apple": "1F3C5", "google": "1F3C5", "twitter": "1F3C5" },
            "shortcode": "sports_medal",
            "category": "activity"
        },
        {
            "name": "first_place_medal",
            "unicode": { "apple": "1F947", "google": "1F947", "twitter": "1F947" },
            "shortcode": "first_place_medal",
            "category": "activity"
        },
        {
            "name": "second_place_medal",
            "unicode": { "apple": "1F948", "google": "1F948", "twitter": "1F948" },
            "shortcode": "second_place_medal",
            "category": "activity"
        },
        {
            "name": "third_place_medal",
            "unicode": { "apple": "1F949", "google": "1F949", "twitter": "1F949" },
            "shortcode": "third_place_medal",
            "category": "activity"
        },
        {
            "name": "soccer",
            "unicode": { "apple": "26BD", "google": "26BD", "twitter": "26BD" },
            "shortcode": "soccer",
            "category": "activity"
        },
        {
            "name": "baseball",
            "unicode": { "apple": "26BE", "google": "26BE", "twitter": "26BE" },
            "shortcode": "baseball",
            "category": "activity"
        },
        {
            "name": "basketball",
            "unicode": { "apple": "1F3C0", "google": "1F3C0", "twitter": "1F3C0" },
            "shortcode": "basketball",
            "category": "activity"
        },
        {
            "name": "volleyball",
            "unicode": { "apple": "1F3D0", "google": "1F3D0", "twitter": "1F3D0" },
            "shortcode": "volleyball",
            "category": "activity"
        },
        {
            "name": "football",
            "unicode": { "apple": "1F3C8", "google": "1F3C8", "twitter": "1F3C8" },
            "shortcode": "football",
            "category": "activity"
        },
        {
            "name": "rugby_football",
            "unicode": { "apple": "1F3C9", "google": "1F3C9", "twitter": "1F3C9" },
            "shortcode": "rugby_football",
            "category": "activity"
        },
        {
            "name": "tennis",
            "unicode": { "apple": "1F3BE", "google": "1F3BE", "twitter": "1F3BE" },
            "shortcode": "tennis",
            "category": "activity"
        },
        {
            "name": "8ball",
            "unicode": { "apple": "1F3B1", "google": "1F3B1", "twitter": "1F3B1" },
            "shortcode": "8ball",
            "category": "activity"
        },
        {
            "name": "bowling",
            "unicode": { "apple": "1F3B3", "google": "1F3B3", "twitter": "1F3B3" },
            "shortcode": "bowling",
            "category": "activity"
        },
        {
            "name": "cricket_bat_and_ball",
            "unicode": { "apple": "1F3CF", "google": "1F3CF", "twitter": "1F3CF" },
            "shortcode": "cricket_bat_and_ball",
            "category": "activity"
        },
        {
            "name": "field_hockey_stick_and_ball",
            "unicode": { "apple": "1F3D1", "google": "1F3D1", "twitter": "1F3D1" },
            "shortcode": "field_hockey_stick_and_ball",
            "category": "activity"
        },
        {
            "name": "ice_hockey_stick_and_puck",
            "unicode": { "apple": "1F3D2", "google": "1F3D2", "twitter": "1F3D2" },
            "shortcode": "ice_hockey_stick_and_puck",
            "category": "activity"
        },
        {
            "name": "table_tennis_paddle_and_ball",
            "unicode": { "apple": "1F3D3", "google": "1F3D3", "twitter": "1F3D3" },
            "shortcode": "table_tennis_paddle_and_ball",
            "category": "activity"
        },
        {
            "name": "badminton_racquet_and_shuttlecock",
            "unicode": { "apple": "1F3F8", "google": "1F3F8", "twitter": "1F3F8" },
            "shortcode": "badminton_racquet_and_shuttlecock",
            "category": "activity"
        },
        {
            "name": "boxing_glove",
            "unicode": { "apple": "1F94A", "google": "1F94A", "twitter": "1F94A" },
            "shortcode": "boxing_glove",
            "category": "activity"
        },
        {
            "name": "martial_arts_uniform",
            "unicode": { "apple": "1F94B", "google": "1F94B", "twitter": "1F94B" },
            "shortcode": "martial_arts_uniform",
            "category": "activity"
        },
        {
            "name": "goal_net",
            "unicode": { "apple": "1F945", "google": "1F945", "twitter": "1F945" },
            "shortcode": "goal_net",
            "category": "activity"
        },
        {
            "name": "dart",
            "unicode": { "apple": "1F3AF", "google": "1F3AF", "twitter": "1F3AF" },
            "shortcode": "dart",
            "category": "activity"
        },
        {
            "name": "golf",
            "unicode": { "apple": "26F3", "google": "26F3", "twitter": "26F3" },
            "shortcode": "golf",
            "category": "activity"
        },
        {
            "name": "ice_skate",
            "unicode": { "apple": "26F8-FE0F", "google": "26F8-FE0F", "twitter": "26F8-FE0F" },
            "shortcode": "ice_skate",
            "category": "activity"
        },
        {
            "name": "fishing_pole_and_fish",
            "unicode": { "apple": "1F3A3", "google": "1F3A3", "twitter": "1F3A3" },
            "shortcode": "fishing_pole_and_fish",
            "category": "activity"
        },
        {
            "name": "running_shirt_with_sash",
            "unicode": { "apple": "1F3BD", "google": "1F3BD", "twitter": "1F3BD" },
            "shortcode": "running_shirt_with_sash",
            "category": "activity"
        },
        {
            "name": "ski",
            "unicode": { "apple": "1F3BF", "google": "1F3BF", "twitter": "1F3BF" },
            "shortcode": "ski",
            "category": "activity"
        },
        {
            "name": "sled",
            "unicode": { "apple": "1F6F7", "google": "1F6F7", "twitter": "1F6F7" },
            "shortcode": "sled",
            "category": "activity"
        },
        {
            "name": "curling_stone",
            "unicode": { "apple": "1F94C", "google": "1F94C", "twitter": "1F94C" },
            "shortcode": "curling_stone",
            "category": "activity"
        },
        {
            "name": "video_game",
            "unicode": { "apple": "1F3AE", "google": "1F3AE", "twitter": "1F3AE" },
            "shortcode": "video_game",
            "category": "activity"
        },
        {
            "name": "joystick",
            "unicode": { "apple": "1F579-FE0F", "google": "1F579-FE0F", "twitter": "1F579-FE0F" },
            "shortcode": "joystick",
            "category": "activity"
        },
        {
            "name": "game_die",
            "unicode": { "apple": "1F3B2", "google": "1F3B2", "twitter": "1F3B2" },
            "shortcode": "game_die",
            "category": "activity"
        },
        {
            "name": "spades",
            "unicode": { "apple": "2660-FE0F", "google": "2660-FE0F", "twitter": "2660-FE0F" },
            "shortcode": "spades",
            "category": "activity"
        },
        {
            "name": "hearts",
            "unicode": { "apple": "2665-FE0F", "google": "2665-FE0F", "twitter": "2665-FE0F" },
            "shortcode": "hearts",
            "category": "activity"
        },
        {
            "name": "diamonds",
            "unicode": { "apple": "2666-FE0F", "google": "2666-FE0F", "twitter": "2666-FE0F" },
            "shortcode": "diamonds",
            "category": "activity"
        },
        {
            "name": "clubs",
            "unicode": { "apple": "2663-FE0F", "google": "2663-FE0F", "twitter": "2663-FE0F" },
            "shortcode": "clubs",
            "category": "activity"
        },
        {
            "name": "black_joker",
            "unicode": { "apple": "1F0CF", "google": "1F0CF", "twitter": "1F0CF" },
            "shortcode": "black_joker",
            "category": "activity"
        },
        {
            "name": "mahjong",
            "unicode": { "apple": "1F004", "google": "1F004", "twitter": "1F004" },
            "shortcode": "mahjong",
            "category": "activity"
        },
        {
            "name": "flower_playing_cards",
            "unicode": { "apple": "1F3B4", "google": "1F3B4", "twitter": "1F3B4" },
            "shortcode": "flower_playing_cards",
            "category": "activity"
        },
        {
            "name": "earth_africa",
            "unicode": { "apple": "1F30D", "google": "1F30D", "twitter": "1F30D" },
            "shortcode": "earth_africa",
            "category": "travel"
        },
        {
            "name": "earth_americas",
            "unicode": { "apple": "1F30E", "google": "1F30E", "twitter": "1F30E" },
            "shortcode": "earth_americas",
            "category": "travel"
        },
        {
            "name": "earth_asia",
            "unicode": { "apple": "1F30F", "google": "1F30F", "twitter": "1F30F" },
            "shortcode": "earth_asia",
            "category": "travel"
        },
        {
            "name": "globe_with_meridians",
            "unicode": { "apple": "1F310", "google": "1F310", "twitter": "1F310" },
            "shortcode": "globe_with_meridians",
            "category": "travel"
        },
        {
            "name": "world_map",
            "unicode": { "apple": "1F5FA-FE0F", "google": "1F5FA-FE0F", "twitter": "1F5FA-FE0F" },
            "shortcode": "world_map",
            "category": "travel"
        },
        {
            "name": "japan",
            "unicode": { "apple": "1F5FE", "google": "1F5FE", "twitter": "1F5FE" },
            "shortcode": "japan",
            "category": "travel"
        },
        {
            "name": "snow_capped_mountain",
            "unicode": { "apple": "1F3D4-FE0F", "google": "1F3D4-FE0F", "twitter": "1F3D4-FE0F" },
            "shortcode": "snow_capped_mountain",
            "category": "travel"
        },
        {
            "name": "mountain",
            "unicode": { "apple": "26F0-FE0F", "google": "26F0-FE0F", "twitter": "26F0-FE0F" },
            "shortcode": "mountain",
            "category": "travel"
        },
        {
            "name": "volcano",
            "unicode": { "apple": "1F30B", "google": "1F30B", "twitter": "1F30B" },
            "shortcode": "volcano",
            "category": "travel"
        },
        {
            "name": "mount_fuji",
            "unicode": { "apple": "1F5FB", "google": "1F5FB", "twitter": "1F5FB" },
            "shortcode": "mount_fuji",
            "category": "travel"
        },
        {
            "name": "camping",
            "unicode": { "apple": "1F3D5-FE0F", "google": "1F3D5-FE0F", "twitter": "1F3D5-FE0F" },
            "shortcode": "camping",
            "category": "travel"
        },
        {
            "name": "beach_with_umbrella",
            "unicode": { "apple": "1F3D6-FE0F", "google": "1F3D6-FE0F", "twitter": "1F3D6-FE0F" },
            "shortcode": "beach_with_umbrella",
            "category": "travel"
        },
        {
            "name": "desert",
            "unicode": { "apple": "1F3DC-FE0F", "google": "1F3DC-FE0F", "twitter": "1F3DC-FE0F" },
            "shortcode": "desert",
            "category": "travel"
        },
        {
            "name": "desert_island",
            "unicode": { "apple": "1F3DD-FE0F", "google": "1F3DD-FE0F", "twitter": "1F3DD-FE0F" },
            "shortcode": "desert_island",
            "category": "travel"
        },
        {
            "name": "national_park",
            "unicode": { "apple": "1F3DE-FE0F", "google": "1F3DE-FE0F", "twitter": "1F3DE-FE0F" },
            "shortcode": "national_park",
            "category": "travel"
        },
        {
            "name": "stadium",
            "unicode": { "apple": "1F3DF-FE0F", "google": "1F3DF-FE0F", "twitter": "1F3DF-FE0F" },
            "shortcode": "stadium",
            "category": "travel"
        },
        {
            "name": "classical_building",
            "unicode": { "apple": "1F3DB-FE0F", "google": "1F3DB-FE0F", "twitter": "1F3DB-FE0F" },
            "shortcode": "classical_building",
            "category": "travel"
        },
        {
            "name": "building_construction",
            "unicode": { "apple": "1F3D7-FE0F", "google": "1F3D7-FE0F", "twitter": "1F3D7-FE0F" },
            "shortcode": "building_construction",
            "category": "travel"
        },
        {
            "name": "house_buildings",
            "unicode": { "apple": "1F3D8-FE0F", "google": "1F3D8-FE0F", "twitter": "1F3D8-FE0F" },
            "shortcode": "house_buildings",
            "category": "travel"
        },
        {
            "name": "cityscape",
            "unicode": { "apple": "1F3D9-FE0F", "google": "1F3D9-FE0F", "twitter": "1F3D9-FE0F" },
            "shortcode": "cityscape",
            "category": "travel"
        },
        {
            "name": "derelict_house_building",
            "unicode": { "apple": "1F3DA-FE0F", "google": "1F3DA-FE0F", "twitter": "1F3DA-FE0F" },
            "shortcode": "derelict_house_building",
            "category": "travel"
        },
        {
            "name": "house",
            "unicode": { "apple": "1F3E0", "google": "1F3E0", "twitter": "1F3E0" },
            "shortcode": "house",
            "category": "travel"
        },
        {
            "name": "house_with_garden",
            "unicode": { "apple": "1F3E1", "google": "1F3E1", "twitter": "1F3E1" },
            "shortcode": "house_with_garden",
            "category": "travel"
        },
        {
            "name": "office",
            "unicode": { "apple": "1F3E2", "google": "1F3E2", "twitter": "1F3E2" },
            "shortcode": "office",
            "category": "travel"
        },
        {
            "name": "post_office",
            "unicode": { "apple": "1F3E3", "google": "1F3E3", "twitter": "1F3E3" },
            "shortcode": "post_office",
            "category": "travel"
        },
        {
            "name": "european_post_office",
            "unicode": { "apple": "1F3E4", "google": "1F3E4", "twitter": "1F3E4" },
            "shortcode": "european_post_office",
            "category": "travel"
        },
        {
            "name": "hospital",
            "unicode": { "apple": "1F3E5", "google": "1F3E5", "twitter": "1F3E5" },
            "shortcode": "hospital",
            "category": "travel"
        },
        {
            "name": "bank",
            "unicode": { "apple": "1F3E6", "google": "1F3E6", "twitter": "1F3E6" },
            "shortcode": "bank",
            "category": "travel"
        },
        {
            "name": "hotel",
            "unicode": { "apple": "1F3E8", "google": "1F3E8", "twitter": "1F3E8" },
            "shortcode": "hotel",
            "category": "travel"
        },
        {
            "name": "love_hotel",
            "unicode": { "apple": "1F3E9", "google": "1F3E9", "twitter": "1F3E9" },
            "shortcode": "love_hotel",
            "category": "travel"
        },
        {
            "name": "convenience_store",
            "unicode": { "apple": "1F3EA", "google": "1F3EA", "twitter": "1F3EA" },
            "shortcode": "convenience_store",
            "category": "travel"
        },
        {
            "name": "school",
            "unicode": { "apple": "1F3EB", "google": "1F3EB", "twitter": "1F3EB" },
            "shortcode": "school",
            "category": "travel"
        },
        {
            "name": "department_store",
            "unicode": { "apple": "1F3EC", "google": "1F3EC", "twitter": "1F3EC" },
            "shortcode": "department_store",
            "category": "travel"
        },
        {
            "name": "factory",
            "unicode": { "apple": "1F3ED", "google": "1F3ED", "twitter": "1F3ED" },
            "shortcode": "factory",
            "category": "travel"
        },
        {
            "name": "japanese_castle",
            "unicode": { "apple": "1F3EF", "google": "1F3EF", "twitter": "1F3EF" },
            "shortcode": "japanese_castle",
            "category": "travel"
        },
        {
            "name": "european_castle",
            "unicode": { "apple": "1F3F0", "google": "1F3F0", "twitter": "1F3F0" },
            "shortcode": "european_castle",
            "category": "travel"
        },
        {
            "name": "wedding",
            "unicode": { "apple": "1F492", "google": "1F492", "twitter": "1F492" },
            "shortcode": "wedding",
            "category": "travel"
        },
        {
            "name": "tokyo_tower",
            "unicode": { "apple": "1F5FC", "google": "1F5FC", "twitter": "1F5FC" },
            "shortcode": "tokyo_tower",
            "category": "travel"
        },
        {
            "name": "statue_of_liberty",
            "unicode": { "apple": "1F5FD", "google": "1F5FD", "twitter": "1F5FD" },
            "shortcode": "statue_of_liberty",
            "category": "travel"
        },
        {
            "name": "church",
            "unicode": { "apple": "26EA", "google": "26EA", "twitter": "26EA" },
            "shortcode": "church",
            "category": "travel"
        },
        {
            "name": "mosque",
            "unicode": { "apple": "1F54C", "google": "1F54C", "twitter": "1F54C" },
            "shortcode": "mosque",
            "category": "travel"
        },
        {
            "name": "synagogue",
            "unicode": { "apple": "1F54D", "google": "1F54D", "twitter": "1F54D" },
            "shortcode": "synagogue",
            "category": "travel"
        },
        {
            "name": "shinto_shrine",
            "unicode": { "apple": "26E9-FE0F", "google": "26E9-FE0F", "twitter": "26E9-FE0F" },
            "shortcode": "shinto_shrine",
            "category": "travel"
        },
        {
            "name": "kaaba",
            "unicode": { "apple": "1F54B", "google": "1F54B", "twitter": "1F54B" },
            "shortcode": "kaaba",
            "category": "travel"
        },
        {
            "name": "fountain",
            "unicode": { "apple": "26F2", "google": "26F2", "twitter": "26F2" },
            "shortcode": "fountain",
            "category": "travel"
        },
        {
            "name": "tent",
            "unicode": { "apple": "26FA", "google": "26FA", "twitter": "26FA" },
            "shortcode": "tent",
            "category": "travel"
        },
        {
            "name": "foggy",
            "unicode": { "apple": "1F301", "google": "1F301", "twitter": "1F301" },
            "shortcode": "foggy",
            "category": "travel"
        },
        {
            "name": "night_with_stars",
            "unicode": { "apple": "1F303", "google": "1F303", "twitter": "1F303" },
            "shortcode": "night_with_stars",
            "category": "travel"
        },
        {
            "name": "sunrise_over_mountains",
            "unicode": { "apple": "1F304", "google": "1F304", "twitter": "1F304" },
            "shortcode": "sunrise_over_mountains",
            "category": "travel"
        },
        {
            "name": "sunrise",
            "unicode": { "apple": "1F305", "google": "1F305", "twitter": "1F305" },
            "shortcode": "sunrise",
            "category": "travel"
        },
        {
            "name": "city_sunset",
            "unicode": { "apple": "1F306", "google": "1F306", "twitter": "1F306" },
            "shortcode": "city_sunset",
            "category": "travel"
        },
        {
            "name": "city_sunrise",
            "unicode": { "apple": "1F307", "google": "1F307", "twitter": "1F307" },
            "shortcode": "city_sunrise",
            "category": "travel"
        },
        {
            "name": "bridge_at_night",
            "unicode": { "apple": "1F309", "google": "1F309", "twitter": "1F309" },
            "shortcode": "bridge_at_night",
            "category": "travel"
        },
        {
            "name": "hotsprings",
            "unicode": { "apple": "2668-FE0F", "google": "2668-FE0F", "twitter": "2668-FE0F" },
            "shortcode": "hotsprings",
            "category": "travel"
        },
        {
            "name": "milky_way",
            "unicode": { "apple": "1F30C", "google": "1F30C", "twitter": "1F30C" },
            "shortcode": "milky_way",
            "category": "travel"
        },
        {
            "name": "carousel_horse",
            "unicode": { "apple": "1F3A0", "google": "1F3A0", "twitter": "1F3A0" },
            "shortcode": "carousel_horse",
            "category": "travel"
        },
        {
            "name": "ferris_wheel",
            "unicode": { "apple": "1F3A1", "google": "1F3A1", "twitter": "1F3A1" },
            "shortcode": "ferris_wheel",
            "category": "travel"
        },
        {
            "name": "roller_coaster",
            "unicode": { "apple": "1F3A2", "google": "1F3A2", "twitter": "1F3A2" },
            "shortcode": "roller_coaster",
            "category": "travel"
        },
        {
            "name": "barber",
            "unicode": { "apple": "1F488", "google": "1F488", "twitter": "1F488" },
            "shortcode": "barber",
            "category": "travel"
        },
        {
            "name": "circus_tent",
            "unicode": { "apple": "1F3AA", "google": "1F3AA", "twitter": "1F3AA" },
            "shortcode": "circus_tent",
            "category": "travel"
        },
        {
            "name": "performing_arts",
            "unicode": { "apple": "1F3AD", "google": "1F3AD", "twitter": "1F3AD" },
            "shortcode": "performing_arts",
            "category": "travel"
        },
        {
            "name": "frame_with_picture",
            "unicode": { "apple": "1F5BC-FE0F", "google": "1F5BC-FE0F", "twitter": "1F5BC-FE0F" },
            "shortcode": "frame_with_picture",
            "category": "travel"
        },
        {
            "name": "art",
            "unicode": { "apple": "1F3A8", "google": "1F3A8", "twitter": "1F3A8" },
            "shortcode": "art",
            "category": "travel"
        },
        {
            "name": "slot_machine",
            "unicode": { "apple": "1F3B0", "google": "1F3B0", "twitter": "1F3B0" },
            "shortcode": "slot_machine",
            "category": "travel"
        },
        {
            "name": "steam_locomotive",
            "unicode": { "apple": "1F682", "google": "1F682", "twitter": "1F682" },
            "shortcode": "steam_locomotive",
            "category": "travel"
        },
        {
            "name": "railway_car",
            "unicode": { "apple": "1F683", "google": "1F683", "twitter": "1F683" },
            "shortcode": "railway_car",
            "category": "travel"
        },
        {
            "name": "bullettrain_side",
            "unicode": { "apple": "1F684", "google": "1F684", "twitter": "1F684" },
            "shortcode": "bullettrain_side",
            "category": "travel"
        },
        {
            "name": "bullettrain_front",
            "unicode": { "apple": "1F685", "google": "1F685", "twitter": "1F685" },
            "shortcode": "bullettrain_front",
            "category": "travel"
        },
        {
            "name": "train2",
            "unicode": { "apple": "1F686", "google": "1F686", "twitter": "1F686" },
            "shortcode": "train2",
            "category": "travel"
        },
        {
            "name": "metro",
            "unicode": { "apple": "1F687", "google": "1F687", "twitter": "1F687" },
            "shortcode": "metro",
            "category": "travel"
        },
        {
            "name": "light_rail",
            "unicode": { "apple": "1F688", "google": "1F688", "twitter": "1F688" },
            "shortcode": "light_rail",
            "category": "travel"
        },
        {
            "name": "station",
            "unicode": { "apple": "1F689", "google": "1F689", "twitter": "1F689" },
            "shortcode": "station",
            "category": "travel"
        },
        {
            "name": "tram",
            "unicode": { "apple": "1F68A", "google": "1F68A", "twitter": "1F68A" },
            "shortcode": "tram",
            "category": "travel"
        },
        {
            "name": "monorail",
            "unicode": { "apple": "1F69D", "google": "1F69D", "twitter": "1F69D" },
            "shortcode": "monorail",
            "category": "travel"
        },
        {
            "name": "mountain_railway",
            "unicode": { "apple": "1F69E", "google": "1F69E", "twitter": "1F69E" },
            "shortcode": "mountain_railway",
            "category": "travel"
        },
        {
            "name": "train",
            "unicode": { "apple": "1F68B", "google": "1F68B", "twitter": "1F68B" },
            "shortcode": "train",
            "category": "travel"
        },
        {
            "name": "bus",
            "unicode": { "apple": "1F68C", "google": "1F68C", "twitter": "1F68C" },
            "shortcode": "bus",
            "category": "travel"
        },
        {
            "name": "oncoming_bus",
            "unicode": { "apple": "1F68D", "google": "1F68D", "twitter": "1F68D" },
            "shortcode": "oncoming_bus",
            "category": "travel"
        },
        {
            "name": "trolleybus",
            "unicode": { "apple": "1F68E", "google": "1F68E", "twitter": "1F68E" },
            "shortcode": "trolleybus",
            "category": "travel"
        },
        {
            "name": "minibus",
            "unicode": { "apple": "1F690", "google": "1F690", "twitter": "1F690" },
            "shortcode": "minibus",
            "category": "travel"
        },
        {
            "name": "ambulance",
            "unicode": { "apple": "1F691", "google": "1F691", "twitter": "1F691" },
            "shortcode": "ambulance",
            "category": "travel"
        },
        {
            "name": "fire_engine",
            "unicode": { "apple": "1F692", "google": "1F692", "twitter": "1F692" },
            "shortcode": "fire_engine",
            "category": "travel"
        },
        {
            "name": "police_car",
            "unicode": { "apple": "1F693", "google": "1F693", "twitter": "1F693" },
            "shortcode": "police_car",
            "category": "travel"
        },
        {
            "name": "oncoming_police_car",
            "unicode": { "apple": "1F694", "google": "1F694", "twitter": "1F694" },
            "shortcode": "oncoming_police_car",
            "category": "travel"
        },
        {
            "name": "taxi",
            "unicode": { "apple": "1F695", "google": "1F695", "twitter": "1F695" },
            "shortcode": "taxi",
            "category": "travel"
        },
        {
            "name": "oncoming_taxi",
            "unicode": { "apple": "1F696", "google": "1F696", "twitter": "1F696" },
            "shortcode": "oncoming_taxi",
            "category": "travel"
        },
        {
            "name": "car",
            "unicode": { "apple": "1F697", "google": "1F697", "twitter": "1F697" },
            "shortcode": "car",
            "category": "travel"
        },
        {
            "name": "oncoming_automobile",
            "unicode": { "apple": "1F698", "google": "1F698", "twitter": "1F698" },
            "shortcode": "oncoming_automobile",
            "category": "travel"
        },
        {
            "name": "blue_car",
            "unicode": { "apple": "1F699", "google": "1F699", "twitter": "1F699" },
            "shortcode": "blue_car",
            "category": "travel"
        },
        {
            "name": "truck",
            "unicode": { "apple": "1F69A", "google": "1F69A", "twitter": "1F69A" },
            "shortcode": "truck",
            "category": "travel"
        },
        {
            "name": "articulated_lorry",
            "unicode": { "apple": "1F69B", "google": "1F69B", "twitter": "1F69B" },
            "shortcode": "articulated_lorry",
            "category": "travel"
        },
        {
            "name": "tractor",
            "unicode": { "apple": "1F69C", "google": "1F69C", "twitter": "1F69C" },
            "shortcode": "tractor",
            "category": "travel"
        },
        {
            "name": "bike",
            "unicode": { "apple": "1F6B2", "google": "1F6B2", "twitter": "1F6B2" },
            "shortcode": "bike",
            "category": "travel"
        },
        {
            "name": "scooter",
            "unicode": { "apple": "1F6F4", "google": "1F6F4", "twitter": "1F6F4" },
            "shortcode": "scooter",
            "category": "travel"
        },
        {
            "name": "motor_scooter",
            "unicode": { "apple": "1F6F5", "google": "1F6F5", "twitter": "1F6F5" },
            "shortcode": "motor_scooter",
            "category": "travel"
        },
        {
            "name": "busstop",
            "unicode": { "apple": "1F68F", "google": "1F68F", "twitter": "1F68F" },
            "shortcode": "busstop",
            "category": "travel"
        },
        {
            "name": "motorway",
            "unicode": { "apple": "1F6E3-FE0F", "google": "1F6E3-FE0F", "twitter": "1F6E3-FE0F" },
            "shortcode": "motorway",
            "category": "travel"
        },
        {
            "name": "railway_track",
            "unicode": { "apple": "1F6E4-FE0F", "google": "1F6E4-FE0F", "twitter": "1F6E4-FE0F" },
            "shortcode": "railway_track",
            "category": "travel"
        },
        {
            "name": "fuelpump",
            "unicode": { "apple": "26FD", "google": "26FD", "twitter": "26FD" },
            "shortcode": "fuelpump",
            "category": "travel"
        },
        {
            "name": "rotating_light",
            "unicode": { "apple": "1F6A8", "google": "1F6A8", "twitter": "1F6A8" },
            "shortcode": "rotating_light",
            "category": "travel"
        },
        {
            "name": "traffic_light",
            "unicode": { "apple": "1F6A5", "google": "1F6A5", "twitter": "1F6A5" },
            "shortcode": "traffic_light",
            "category": "travel"
        },
        {
            "name": "vertical_traffic_light",
            "unicode": { "apple": "1F6A6", "google": "1F6A6", "twitter": "1F6A6" },
            "shortcode": "vertical_traffic_light",
            "category": "travel"
        },
        {
            "name": "construction",
            "unicode": { "apple": "1F6A7", "google": "1F6A7", "twitter": "1F6A7" },
            "shortcode": "construction",
            "category": "travel"
        },
        {
            "name": "octagonal_sign",
            "unicode": { "apple": "1F6D1", "google": "1F6D1", "twitter": "1F6D1" },
            "shortcode": "octagonal_sign",
            "category": "travel"
        },
        {
            "name": "anchor",
            "unicode": { "apple": "2693", "google": "2693", "twitter": "2693" },
            "shortcode": "anchor",
            "category": "travel"
        },
        {
            "name": "boat",
            "unicode": { "apple": "26F5", "google": "26F5", "twitter": "26F5" },
            "shortcode": "boat",
            "category": "travel"
        },
        {
            "name": "canoe",
            "unicode": { "apple": "1F6F6", "google": "1F6F6", "twitter": "1F6F6" },
            "shortcode": "canoe",
            "category": "travel"
        },
        {
            "name": "speedboat",
            "unicode": { "apple": "1F6A4", "google": "1F6A4", "twitter": "1F6A4" },
            "shortcode": "speedboat",
            "category": "travel"
        },
        {
            "name": "passenger_ship",
            "unicode": { "apple": "1F6F3-FE0F", "google": "1F6F3-FE0F", "twitter": "1F6F3-FE0F" },
            "shortcode": "passenger_ship",
            "category": "travel"
        },
        {
            "name": "ferry",
            "unicode": { "apple": "26F4-FE0F", "google": "26F4-FE0F", "twitter": "26F4-FE0F" },
            "shortcode": "ferry",
            "category": "travel"
        },
        {
            "name": "motor_boat",
            "unicode": { "apple": "1F6E5-FE0F", "google": "1F6E5-FE0F", "twitter": "1F6E5-FE0F" },
            "shortcode": "motor_boat",
            "category": "travel"
        },
        {
            "name": "ship",
            "unicode": { "apple": "1F6A2", "google": "1F6A2", "twitter": "1F6A2" },
            "shortcode": "ship",
            "category": "travel"
        },
        {
            "name": "airplane",
            "unicode": { "apple": "2708-FE0F", "google": "2708-FE0F", "twitter": "2708-FE0F" },
            "shortcode": "airplane",
            "category": "travel"
        },
        {
            "name": "small_airplane",
            "unicode": { "apple": "1F6E9-FE0F", "google": "1F6E9-FE0F", "twitter": "1F6E9-FE0F" },
            "shortcode": "small_airplane",
            "category": "travel"
        },
        {
            "name": "airplane_departure",
            "unicode": { "apple": "1F6EB", "google": "1F6EB", "twitter": "1F6EB" },
            "shortcode": "airplane_departure",
            "category": "travel"
        },
        {
            "name": "airplane_arriving",
            "unicode": { "apple": "1F6EC", "google": "1F6EC", "twitter": "1F6EC" },
            "shortcode": "airplane_arriving",
            "category": "travel"
        },
        {
            "name": "seat",
            "unicode": { "apple": "1F4BA", "google": "1F4BA", "twitter": "1F4BA" },
            "shortcode": "seat",
            "category": "travel"
        },
        {
            "name": "helicopter",
            "unicode": { "apple": "1F681", "google": "1F681", "twitter": "1F681" },
            "shortcode": "helicopter",
            "category": "travel"
        },
        {
            "name": "suspension_railway",
            "unicode": { "apple": "1F69F", "google": "1F69F", "twitter": "1F69F" },
            "shortcode": "suspension_railway",
            "category": "travel"
        },
        {
            "name": "mountain_cableway",
            "unicode": { "apple": "1F6A0", "google": "1F6A0", "twitter": "1F6A0" },
            "shortcode": "mountain_cableway",
            "category": "travel"
        },
        {
            "name": "aerial_tramway",
            "unicode": { "apple": "1F6A1", "google": "1F6A1", "twitter": "1F6A1" },
            "shortcode": "aerial_tramway",
            "category": "travel"
        },
        {
            "name": "satellite",
            "unicode": { "apple": "1F6F0-FE0F", "google": "1F6F0-FE0F", "twitter": "1F6F0-FE0F" },
            "shortcode": "satellite",
            "category": "travel"
        },
        {
            "name": "rocket",
            "unicode": { "apple": "1F680", "google": "1F680", "twitter": "1F680" },
            "shortcode": "rocket",
            "category": "travel"
        },
        {
            "name": "flying_saucer",
            "unicode": { "apple": "1F6F8", "google": "1F6F8", "twitter": "1F6F8" },
            "shortcode": "flying_saucer",
            "category": "travel"
        },
        {
            "name": "bellhop_bell",
            "unicode": { "apple": "1F6CE-FE0F", "google": "1F6CE-FE0F", "twitter": "1F6CE-FE0F" },
            "shortcode": "bellhop_bell",
            "category": "travel"
        },
        {
            "name": "door",
            "unicode": { "apple": "1F6AA", "google": "1F6AA", "twitter": "1F6AA" },
            "shortcode": "door",
            "category": "travel"
        },
        {
            "name": "bed",
            "unicode": { "apple": "1F6CF-FE0F", "google": "1F6CF-FE0F", "twitter": "1F6CF-FE0F" },
            "shortcode": "bed",
            "category": "travel"
        },
        {
            "name": "couch_and_lamp",
            "unicode": { "apple": "1F6CB-FE0F", "google": "1F6CB-FE0F", "twitter": "1F6CB-FE0F" },
            "shortcode": "couch_and_lamp",
            "category": "travel"
        },
        {
            "name": "toilet",
            "unicode": { "apple": "1F6BD", "google": "1F6BD", "twitter": "1F6BD" },
            "shortcode": "toilet",
            "category": "travel"
        },
        {
            "name": "shower",
            "unicode": { "apple": "1F6BF", "google": "1F6BF", "twitter": "1F6BF" },
            "shortcode": "shower",
            "category": "travel"
        },
        {
            "name": "bathtub",
            "unicode": { "apple": "1F6C1", "google": "1F6C1", "twitter": "1F6C1" },
            "shortcode": "bathtub",
            "category": "travel"
        },
        {
            "name": "hourglass",
            "unicode": { "apple": "231B", "google": "231B", "twitter": "231B" },
            "shortcode": "hourglass",
            "category": "travel"
        },
        {
            "name": "hourglass_flowing_sand",
            "unicode": { "apple": "23F3", "google": "23F3", "twitter": "23F3" },
            "shortcode": "hourglass_flowing_sand",
            "category": "travel"
        },
        {
            "name": "watch",
            "unicode": { "apple": "231A", "google": "231A", "twitter": "231A" },
            "shortcode": "watch",
            "category": "travel"
        },
        {
            "name": "alarm_clock",
            "unicode": { "apple": "23F0", "google": "23F0", "twitter": "23F0" },
            "shortcode": "alarm_clock",
            "category": "travel"
        },
        {
            "name": "stopwatch",
            "unicode": { "apple": "23F1-FE0F", "google": "23F1-FE0F", "twitter": "23F1-FE0F" },
            "shortcode": "stopwatch",
            "category": "travel"
        },
        {
            "name": "timer_clock",
            "unicode": { "apple": "23F2-FE0F", "google": "23F2-FE0F", "twitter": "23F2-FE0F" },
            "shortcode": "timer_clock",
            "category": "travel"
        },
        {
            "name": "mantelpiece_clock",
            "unicode": { "apple": "1F570-FE0F", "google": "1F570-FE0F", "twitter": "1F570-FE0F" },
            "shortcode": "mantelpiece_clock",
            "category": "travel"
        },
        {
            "name": "clock12",
            "unicode": { "apple": "1F55B", "google": "1F55B", "twitter": "1F55B" },
            "shortcode": "clock12",
            "category": "travel"
        },
        {
            "name": "clock1230",
            "unicode": { "apple": "1F567", "google": "1F567", "twitter": "1F567" },
            "shortcode": "clock1230",
            "category": "travel"
        },
        {
            "name": "clock1",
            "unicode": { "apple": "1F550", "google": "1F550", "twitter": "1F550" },
            "shortcode": "clock1",
            "category": "travel"
        },
        {
            "name": "clock130",
            "unicode": { "apple": "1F55C", "google": "1F55C", "twitter": "1F55C" },
            "shortcode": "clock130",
            "category": "travel"
        },
        {
            "name": "clock2",
            "unicode": { "apple": "1F551", "google": "1F551", "twitter": "1F551" },
            "shortcode": "clock2",
            "category": "travel"
        },
        {
            "name": "clock230",
            "unicode": { "apple": "1F55D", "google": "1F55D", "twitter": "1F55D" },
            "shortcode": "clock230",
            "category": "travel"
        },
        {
            "name": "clock3",
            "unicode": { "apple": "1F552", "google": "1F552", "twitter": "1F552" },
            "shortcode": "clock3",
            "category": "travel"
        },
        {
            "name": "clock330",
            "unicode": { "apple": "1F55E", "google": "1F55E", "twitter": "1F55E" },
            "shortcode": "clock330",
            "category": "travel"
        },
        {
            "name": "clock4",
            "unicode": { "apple": "1F553", "google": "1F553", "twitter": "1F553" },
            "shortcode": "clock4",
            "category": "travel"
        },
        {
            "name": "clock430",
            "unicode": { "apple": "1F55F", "google": "1F55F", "twitter": "1F55F" },
            "shortcode": "clock430",
            "category": "travel"
        },
        {
            "name": "clock5",
            "unicode": { "apple": "1F554", "google": "1F554", "twitter": "1F554" },
            "shortcode": "clock5",
            "category": "travel"
        },
        {
            "name": "clock530",
            "unicode": { "apple": "1F560", "google": "1F560", "twitter": "1F560" },
            "shortcode": "clock530",
            "category": "travel"
        },
        {
            "name": "clock6",
            "unicode": { "apple": "1F555", "google": "1F555", "twitter": "1F555" },
            "shortcode": "clock6",
            "category": "travel"
        },
        {
            "name": "clock630",
            "unicode": { "apple": "1F561", "google": "1F561", "twitter": "1F561" },
            "shortcode": "clock630",
            "category": "travel"
        },
        {
            "name": "clock7",
            "unicode": { "apple": "1F556", "google": "1F556", "twitter": "1F556" },
            "shortcode": "clock7",
            "category": "travel"
        },
        {
            "name": "clock730",
            "unicode": { "apple": "1F562", "google": "1F562", "twitter": "1F562" },
            "shortcode": "clock730",
            "category": "travel"
        },
        {
            "name": "clock8",
            "unicode": { "apple": "1F557", "google": "1F557", "twitter": "1F557" },
            "shortcode": "clock8",
            "category": "travel"
        },
        {
            "name": "clock830",
            "unicode": { "apple": "1F563", "google": "1F563", "twitter": "1F563" },
            "shortcode": "clock830",
            "category": "travel"
        },
        {
            "name": "clock9",
            "unicode": { "apple": "1F558", "google": "1F558", "twitter": "1F558" },
            "shortcode": "clock9",
            "category": "travel"
        },
        {
            "name": "clock930",
            "unicode": { "apple": "1F564", "google": "1F564", "twitter": "1F564" },
            "shortcode": "clock930",
            "category": "travel"
        },
        {
            "name": "clock10",
            "unicode": { "apple": "1F559", "google": "1F559", "twitter": "1F559" },
            "shortcode": "clock10",
            "category": "travel"
        },
        {
            "name": "clock1030",
            "unicode": { "apple": "1F565", "google": "1F565", "twitter": "1F565" },
            "shortcode": "clock1030",
            "category": "travel"
        },
        {
            "name": "clock11",
            "unicode": { "apple": "1F55A", "google": "1F55A", "twitter": "1F55A" },
            "shortcode": "clock11",
            "category": "travel"
        },
        {
            "name": "clock1130",
            "unicode": { "apple": "1F566", "google": "1F566", "twitter": "1F566" },
            "shortcode": "clock1130",
            "category": "travel"
        },
        {
            "name": "new_moon",
            "unicode": { "apple": "1F311", "google": "1F311", "twitter": "1F311" },
            "shortcode": "new_moon",
            "category": "travel"
        },
        {
            "name": "waxing_crescent_moon",
            "unicode": { "apple": "1F312", "google": "1F312", "twitter": "1F312" },
            "shortcode": "waxing_crescent_moon",
            "category": "travel"
        },
        {
            "name": "first_quarter_moon",
            "unicode": { "apple": "1F313", "google": "1F313", "twitter": "1F313" },
            "shortcode": "first_quarter_moon",
            "category": "travel"
        },
        {
            "name": "moon",
            "unicode": { "apple": "1F314", "google": "1F314", "twitter": "1F314" },
            "shortcode": "moon",
            "category": "travel"
        },
        {
            "name": "full_moon",
            "unicode": { "apple": "1F315", "google": "1F315", "twitter": "1F315" },
            "shortcode": "full_moon",
            "category": "travel"
        },
        {
            "name": "waning_gibbous_moon",
            "unicode": { "apple": "1F316", "google": "1F316", "twitter": "1F316" },
            "shortcode": "waning_gibbous_moon",
            "category": "travel"
        },
        {
            "name": "last_quarter_moon",
            "unicode": { "apple": "1F317", "google": "1F317", "twitter": "1F317" },
            "shortcode": "last_quarter_moon",
            "category": "travel"
        },
        {
            "name": "waning_crescent_moon",
            "unicode": { "apple": "1F318", "google": "1F318", "twitter": "1F318" },
            "shortcode": "waning_crescent_moon",
            "category": "travel"
        },
        {
            "name": "crescent_moon",
            "unicode": { "apple": "1F319", "google": "1F319", "twitter": "1F319" },
            "shortcode": "crescent_moon",
            "category": "travel"
        },
        {
            "name": "new_moon_with_face",
            "unicode": { "apple": "1F31A", "google": "1F31A", "twitter": "1F31A" },
            "shortcode": "new_moon_with_face",
            "category": "travel"
        },
        {
            "name": "first_quarter_moon_with_face",
            "unicode": { "apple": "1F31B", "google": "1F31B", "twitter": "1F31B" },
            "shortcode": "first_quarter_moon_with_face",
            "category": "travel"
        },
        {
            "name": "last_quarter_moon_with_face",
            "unicode": { "apple": "1F31C", "google": "1F31C", "twitter": "1F31C" },
            "shortcode": "last_quarter_moon_with_face",
            "category": "travel"
        },
        {
            "name": "thermometer",
            "unicode": { "apple": "1F321-FE0F", "google": "1F321-FE0F", "twitter": "1F321-FE0F" },
            "shortcode": "thermometer",
            "category": "travel"
        },
        {
            "name": "sunny",
            "unicode": { "apple": "2600-FE0F", "google": "2600-FE0F", "twitter": "2600-FE0F" },
            "shortcode": "sunny",
            "category": "travel"
        },
        {
            "name": "full_moon_with_face",
            "unicode": { "apple": "1F31D", "google": "1F31D", "twitter": "1F31D" },
            "shortcode": "full_moon_with_face",
            "category": "travel"
        },
        {
            "name": "sun_with_face",
            "unicode": { "apple": "1F31E", "google": "1F31E", "twitter": "1F31E" },
            "shortcode": "sun_with_face",
            "category": "travel"
        },
        {
            "name": "star",
            "unicode": { "apple": "2B50", "google": "2B50", "twitter": "2B50" },
            "shortcode": "star",
            "category": "travel"
        },
        {
            "name": "star2",
            "unicode": { "apple": "1F31F", "google": "1F31F", "twitter": "1F31F" },
            "shortcode": "star2",
            "category": "travel"
        },
        {
            "name": "stars",
            "unicode": { "apple": "1F320", "google": "1F320", "twitter": "1F320" },
            "shortcode": "stars",
            "category": "travel"
        },
        {
            "name": "cloud",
            "unicode": { "apple": "2601-FE0F", "google": "2601-FE0F", "twitter": "2601-FE0F" },
            "shortcode": "cloud",
            "category": "travel"
        },
        {
            "name": "partly_sunny",
            "unicode": { "apple": "26C5", "google": "26C5", "twitter": "26C5" },
            "shortcode": "partly_sunny",
            "category": "travel"
        },
        {
            "name": "thunder_cloud_and_rain",
            "unicode": { "apple": "26C8-FE0F", "google": "26C8-FE0F", "twitter": "26C8-FE0F" },
            "shortcode": "thunder_cloud_and_rain",
            "category": "travel"
        },
        {
            "name": "mostly_sunny",
            "unicode": { "apple": "1F324-FE0F", "google": "1F324-FE0F", "twitter": "1F324-FE0F" },
            "shortcode": "mostly_sunny",
            "category": "travel"
        },
        {
            "name": "barely_sunny",
            "unicode": { "apple": "1F325-FE0F", "google": "1F325-FE0F", "twitter": "1F325-FE0F" },
            "shortcode": "barely_sunny",
            "category": "travel"
        },
        {
            "name": "partly_sunny_rain",
            "unicode": { "apple": "1F326-FE0F", "google": "1F326-FE0F", "twitter": "1F326-FE0F" },
            "shortcode": "partly_sunny_rain",
            "category": "travel"
        },
        {
            "name": "rain_cloud",
            "unicode": { "apple": "1F327-FE0F", "google": "1F327-FE0F", "twitter": "1F327-FE0F" },
            "shortcode": "rain_cloud",
            "category": "travel"
        },
        {
            "name": "snow_cloud",
            "unicode": { "apple": "1F328-FE0F", "google": "1F328-FE0F", "twitter": "1F328-FE0F" },
            "shortcode": "snow_cloud",
            "category": "travel"
        },
        {
            "name": "lightning",
            "unicode": { "apple": "1F329-FE0F", "google": "1F329-FE0F", "twitter": "1F329-FE0F" },
            "shortcode": "lightning",
            "category": "travel"
        },
        {
            "name": "tornado",
            "unicode": { "apple": "1F32A-FE0F", "google": "1F32A-FE0F", "twitter": "1F32A-FE0F" },
            "shortcode": "tornado",
            "category": "travel"
        },
        {
            "name": "fog",
            "unicode": { "apple": "1F32B-FE0F", "google": "1F32B-FE0F", "twitter": "1F32B-FE0F" },
            "shortcode": "fog",
            "category": "travel"
        },
        {
            "name": "wind_blowing_face",
            "unicode": { "apple": "1F32C-FE0F", "google": "1F32C-FE0F", "twitter": "1F32C-FE0F" },
            "shortcode": "wind_blowing_face",
            "category": "travel"
        },
        {
            "name": "cyclone",
            "unicode": { "apple": "1F300", "google": "1F300", "twitter": "1F300" },
            "shortcode": "cyclone",
            "category": "travel"
        },
        {
            "name": "rainbow",
            "unicode": { "apple": "1F308", "google": "1F308", "twitter": "1F308" },
            "shortcode": "rainbow",
            "category": "travel"
        },
        {
            "name": "closed_umbrella",
            "unicode": { "apple": "1F302", "google": "1F302", "twitter": "1F302" },
            "shortcode": "closed_umbrella",
            "category": "travel"
        },
        {
            "name": "umbrella",
            "unicode": { "apple": "2602-FE0F", "google": "2602-FE0F", "twitter": "2602-FE0F" },
            "shortcode": "umbrella",
            "category": "travel"
        },
        {
            "name": "umbrella_with_rain_drops",
            "unicode": { "apple": "2614", "google": "2614", "twitter": "2614" },
            "shortcode": "umbrella_with_rain_drops",
            "category": "travel"
        },
        {
            "name": "umbrella_on_ground",
            "unicode": { "apple": "26F1-FE0F", "google": "26F1-FE0F", "twitter": "26F1-FE0F" },
            "shortcode": "umbrella_on_ground",
            "category": "travel"
        },
        {
            "name": "zap",
            "unicode": { "apple": "26A1", "google": "26A1", "twitter": "26A1" },
            "shortcode": "zap",
            "category": "travel"
        },
        {
            "name": "snowflake",
            "unicode": { "apple": "2744-FE0F", "google": "2744-FE0F", "twitter": "2744-FE0F" },
            "shortcode": "snowflake",
            "category": "travel"
        },
        {
            "name": "snowman",
            "unicode": { "apple": "2603-FE0F", "google": "2603-FE0F", "twitter": "2603-FE0F" },
            "shortcode": "snowman",
            "category": "travel"
        },
        {
            "name": "snowman_without_snow",
            "unicode": { "apple": "26C4", "google": "26C4", "twitter": "26C4" },
            "shortcode": "snowman_without_snow",
            "category": "travel"
        },
        {
            "name": "comet",
            "unicode": { "apple": "2604-FE0F", "google": "2604-FE0F", "twitter": "2604-FE0F" },
            "shortcode": "comet",
            "category": "travel"
        },
        {
            "name": "fire",
            "unicode": { "apple": "1F525", "google": "1F525", "twitter": "1F525" },
            "shortcode": "fire",
            "category": "travel"
        },
        {
            "name": "droplet",
            "unicode": { "apple": "1F4A7", "google": "1F4A7", "twitter": "1F4A7" },
            "shortcode": "droplet",
            "category": "travel"
        },
        {
            "name": "ocean",
            "unicode": { "apple": "1F30A", "google": "1F30A", "twitter": "1F30A" },
            "shortcode": "ocean",
            "category": "travel"
        },
        {
            "name": "mute",
            "unicode": { "apple": "1F507", "google": "1F507", "twitter": "1F507" },
            "shortcode": "mute",
            "category": "object"
        },
        {
            "name": "speaker",
            "unicode": { "apple": "1F508", "google": "1F508", "twitter": "1F508" },
            "shortcode": "speaker",
            "category": "object"
        },
        {
            "name": "sound",
            "unicode": { "apple": "1F509", "google": "1F509", "twitter": "1F509" },
            "shortcode": "sound",
            "category": "object"
        },
        {
            "name": "loud_sound",
            "unicode": { "apple": "1F50A", "google": "1F50A", "twitter": "1F50A" },
            "shortcode": "loud_sound",
            "category": "object"
        },
        {
            "name": "loudspeaker",
            "unicode": { "apple": "1F4E2", "google": "1F4E2", "twitter": "1F4E2" },
            "shortcode": "loudspeaker",
            "category": "object"
        },
        {
            "name": "mega",
            "unicode": { "apple": "1F4E3", "google": "1F4E3", "twitter": "1F4E3" },
            "shortcode": "mega",
            "category": "object"
        },
        {
            "name": "postal_horn",
            "unicode": { "apple": "1F4EF", "google": "1F4EF", "twitter": "1F4EF" },
            "shortcode": "postal_horn",
            "category": "object"
        },
        {
            "name": "bell",
            "unicode": { "apple": "1F514", "google": "1F514", "twitter": "1F514" },
            "shortcode": "bell",
            "category": "object"
        },
        {
            "name": "no_bell",
            "unicode": { "apple": "1F515", "google": "1F515", "twitter": "1F515" },
            "shortcode": "no_bell",
            "category": "object"
        },
        {
            "name": "musical_score",
            "unicode": { "apple": "1F3BC", "google": "1F3BC", "twitter": "1F3BC" },
            "shortcode": "musical_score",
            "category": "object"
        },
        {
            "name": "musical_note",
            "unicode": { "apple": "1F3B5", "google": "1F3B5", "twitter": "1F3B5" },
            "shortcode": "musical_note",
            "category": "object"
        },
        {
            "name": "notes",
            "unicode": { "apple": "1F3B6", "google": "1F3B6", "twitter": "1F3B6" },
            "shortcode": "notes",
            "category": "object"
        },
        {
            "name": "studio_microphone",
            "unicode": { "apple": "1F399-FE0F", "google": "1F399-FE0F", "twitter": "1F399-FE0F" },
            "shortcode": "studio_microphone",
            "category": "object"
        },
        {
            "name": "level_slider",
            "unicode": { "apple": "1F39A-FE0F", "google": "1F39A-FE0F", "twitter": "1F39A-FE0F" },
            "shortcode": "level_slider",
            "category": "object"
        },
        {
            "name": "control_knobs",
            "unicode": { "apple": "1F39B-FE0F", "google": "1F39B-FE0F", "twitter": "1F39B-FE0F" },
            "shortcode": "control_knobs",
            "category": "object"
        },
        {
            "name": "microphone",
            "unicode": { "apple": "1F3A4", "google": "1F3A4", "twitter": "1F3A4" },
            "shortcode": "microphone",
            "category": "object"
        },
        {
            "name": "headphones",
            "unicode": { "apple": "1F3A7", "google": "1F3A7", "twitter": "1F3A7" },
            "shortcode": "headphones",
            "category": "object"
        },
        {
            "name": "radio",
            "unicode": { "apple": "1F4FB", "google": "1F4FB", "twitter": "1F4FB" },
            "shortcode": "radio",
            "category": "object"
        },
        {
            "name": "saxophone",
            "unicode": { "apple": "1F3B7", "google": "1F3B7", "twitter": "1F3B7" },
            "shortcode": "saxophone",
            "category": "object"
        },
        {
            "name": "guitar",
            "unicode": { "apple": "1F3B8", "google": "1F3B8", "twitter": "1F3B8" },
            "shortcode": "guitar",
            "category": "object"
        },
        {
            "name": "musical_keyboard",
            "unicode": { "apple": "1F3B9", "google": "1F3B9", "twitter": "1F3B9" },
            "shortcode": "musical_keyboard",
            "category": "object"
        },
        {
            "name": "trumpet",
            "unicode": { "apple": "1F3BA", "google": "1F3BA", "twitter": "1F3BA" },
            "shortcode": "trumpet",
            "category": "object"
        },
        {
            "name": "violin",
            "unicode": { "apple": "1F3BB", "google": "1F3BB", "twitter": "1F3BB" },
            "shortcode": "violin",
            "category": "object"
        },
        {
            "name": "drum_with_drumsticks",
            "unicode": { "apple": "1F941", "google": "1F941", "twitter": "1F941" },
            "shortcode": "drum_with_drumsticks",
            "category": "object"
        },
        {
            "name": "iphone",
            "unicode": { "apple": "1F4F1", "google": "1F4F1", "twitter": "1F4F1" },
            "shortcode": "iphone",
            "category": "object"
        },
        {
            "name": "calling",
            "unicode": { "apple": "1F4F2", "google": "1F4F2", "twitter": "1F4F2" },
            "shortcode": "calling",
            "category": "object"
        },
        {
            "name": "phone",
            "unicode": { "apple": "260E-FE0F", "google": "260E-FE0F", "twitter": "260E-FE0F" },
            "shortcode": "phone",
            "category": "object"
        },
        {
            "name": "telephone_receiver",
            "unicode": { "apple": "1F4DE", "google": "1F4DE", "twitter": "1F4DE" },
            "shortcode": "telephone_receiver",
            "category": "object"
        },
        {
            "name": "pager",
            "unicode": { "apple": "1F4DF", "google": "1F4DF", "twitter": "1F4DF" },
            "shortcode": "pager",
            "category": "object"
        },
        {
            "name": "fax",
            "unicode": { "apple": "1F4E0", "google": "1F4E0", "twitter": "1F4E0" },
            "shortcode": "fax",
            "category": "object"
        },
        {
            "name": "battery",
            "unicode": { "apple": "1F50B", "google": "1F50B", "twitter": "1F50B" },
            "shortcode": "battery",
            "category": "object"
        },
        {
            "name": "electric_plug",
            "unicode": { "apple": "1F50C", "google": "1F50C", "twitter": "1F50C" },
            "shortcode": "electric_plug",
            "category": "object"
        },
        {
            "name": "computer",
            "unicode": { "apple": "1F4BB", "google": "1F4BB", "twitter": "1F4BB" },
            "shortcode": "computer",
            "category": "object"
        },
        {
            "name": "desktop_computer",
            "unicode": { "apple": "1F5A5-FE0F", "google": "1F5A5-FE0F", "twitter": "1F5A5-FE0F" },
            "shortcode": "desktop_computer",
            "category": "object"
        },
        {
            "name": "printer",
            "unicode": { "apple": "1F5A8-FE0F", "google": "1F5A8-FE0F", "twitter": "1F5A8-FE0F" },
            "shortcode": "printer",
            "category": "object"
        },
        {
            "name": "keyboard",
            "unicode": { "apple": "2328-FE0F", "google": "2328-FE0F", "twitter": "2328-FE0F" },
            "shortcode": "keyboard",
            "category": "object"
        },
        {
            "name": "three_button_mouse",
            "unicode": { "apple": "1F5B1-FE0F", "google": "1F5B1-FE0F", "twitter": "1F5B1-FE0F" },
            "shortcode": "three_button_mouse",
            "category": "object"
        },
        {
            "name": "trackball",
            "unicode": { "apple": "1F5B2-FE0F", "google": "1F5B2-FE0F", "twitter": "1F5B2-FE0F" },
            "shortcode": "trackball",
            "category": "object"
        },
        {
            "name": "minidisc",
            "unicode": { "apple": "1F4BD", "google": "1F4BD", "twitter": "1F4BD" },
            "shortcode": "minidisc",
            "category": "object"
        },
        {
            "name": "floppy_disk",
            "unicode": { "apple": "1F4BE", "google": "1F4BE", "twitter": "1F4BE" },
            "shortcode": "floppy_disk",
            "category": "object"
        },
        {
            "name": "cd",
            "unicode": { "apple": "1F4BF", "google": "1F4BF", "twitter": "1F4BF" },
            "shortcode": "cd",
            "category": "object"
        },
        {
            "name": "dvd",
            "unicode": { "apple": "1F4C0", "google": "1F4C0", "twitter": "1F4C0" },
            "shortcode": "dvd",
            "category": "object"
        },
        {
            "name": "movie_camera",
            "unicode": { "apple": "1F3A5", "google": "1F3A5", "twitter": "1F3A5" },
            "shortcode": "movie_camera",
            "category": "object"
        },
        {
            "name": "film_frames",
            "unicode": { "apple": "1F39E-FE0F", "google": "1F39E-FE0F", "twitter": "1F39E-FE0F" },
            "shortcode": "film_frames",
            "category": "object"
        },
        {
            "name": "film_projector",
            "unicode": { "apple": "1F4FD-FE0F", "google": "1F4FD-FE0F", "twitter": "1F4FD-FE0F" },
            "shortcode": "film_projector",
            "category": "object"
        },
        {
            "name": "clapper",
            "unicode": { "apple": "1F3AC", "google": "1F3AC", "twitter": "1F3AC" },
            "shortcode": "clapper",
            "category": "object"
        },
        {
            "name": "tv",
            "unicode": { "apple": "1F4FA", "google": "1F4FA", "twitter": "1F4FA" },
            "shortcode": "tv",
            "category": "object"
        },
        {
            "name": "camera",
            "unicode": { "apple": "1F4F7", "google": "1F4F7", "twitter": "1F4F7" },
            "shortcode": "camera",
            "category": "object"
        },
        {
            "name": "camera_with_flash",
            "unicode": { "apple": "1F4F8", "google": "1F4F8", "twitter": "1F4F8" },
            "shortcode": "camera_with_flash",
            "category": "object"
        },
        {
            "name": "video_camera",
            "unicode": { "apple": "1F4F9", "google": "1F4F9", "twitter": "1F4F9" },
            "shortcode": "video_camera",
            "category": "object"
        },
        {
            "name": "vhs",
            "unicode": { "apple": "1F4FC", "google": "1F4FC", "twitter": "1F4FC" },
            "shortcode": "vhs",
            "category": "object"
        },
        {
            "name": "mag",
            "unicode": { "apple": "1F50D", "google": "1F50D", "twitter": "1F50D" },
            "shortcode": "mag",
            "category": "object"
        },
        {
            "name": "mag_right",
            "unicode": { "apple": "1F50E", "google": "1F50E", "twitter": "1F50E" },
            "shortcode": "mag_right",
            "category": "object"
        },
        {
            "name": "microscope",
            "unicode": { "apple": "1F52C", "google": "1F52C", "twitter": "1F52C" },
            "shortcode": "microscope",
            "category": "object"
        },
        {
            "name": "telescope",
            "unicode": { "apple": "1F52D", "google": "1F52D", "twitter": "1F52D" },
            "shortcode": "telescope",
            "category": "object"
        },
        {
            "name": "satellite_antenna",
            "unicode": { "apple": "1F4E1", "google": "1F4E1", "twitter": "1F4E1" },
            "shortcode": "satellite_antenna",
            "category": "object"
        },
        {
            "name": "candle",
            "unicode": { "apple": "1F56F-FE0F", "google": "1F56F-FE0F", "twitter": "1F56F-FE0F" },
            "shortcode": "candle",
            "category": "object"
        },
        {
            "name": "bulb",
            "unicode": { "apple": "1F4A1", "google": "1F4A1", "twitter": "1F4A1" },
            "shortcode": "bulb",
            "category": "object"
        },
        {
            "name": "flashlight",
            "unicode": { "apple": "1F526", "google": "1F526", "twitter": "1F526" },
            "shortcode": "flashlight",
            "category": "object"
        },
        {
            "name": "izakaya_lantern",
            "unicode": { "apple": "1F3EE", "google": "1F3EE", "twitter": "1F3EE" },
            "shortcode": "izakaya_lantern",
            "category": "object"
        },
        {
            "name": "notebook_with_decorative_cover",
            "unicode": { "apple": "1F4D4", "google": "1F4D4", "twitter": "1F4D4" },
            "shortcode": "notebook_with_decorative_cover",
            "category": "object"
        },
        {
            "name": "closed_book",
            "unicode": { "apple": "1F4D5", "google": "1F4D5", "twitter": "1F4D5" },
            "shortcode": "closed_book",
            "category": "object"
        },
        {
            "name": "book",
            "unicode": { "apple": "1F4D6", "google": "1F4D6", "twitter": "1F4D6" },
            "shortcode": "book",
            "category": "object"
        },
        {
            "name": "green_book",
            "unicode": { "apple": "1F4D7", "google": "1F4D7", "twitter": "1F4D7" },
            "shortcode": "green_book",
            "category": "object"
        },
        {
            "name": "blue_book",
            "unicode": { "apple": "1F4D8", "google": "1F4D8", "twitter": "1F4D8" },
            "shortcode": "blue_book",
            "category": "object"
        },
        {
            "name": "orange_book",
            "unicode": { "apple": "1F4D9", "google": "1F4D9", "twitter": "1F4D9" },
            "shortcode": "orange_book",
            "category": "object"
        },
        {
            "name": "books",
            "unicode": { "apple": "1F4DA", "google": "1F4DA", "twitter": "1F4DA" },
            "shortcode": "books",
            "category": "object"
        },
        {
            "name": "notebook",
            "unicode": { "apple": "1F4D3", "google": "1F4D3", "twitter": "1F4D3" },
            "shortcode": "notebook",
            "category": "object"
        },
        {
            "name": "ledger",
            "unicode": { "apple": "1F4D2", "google": "1F4D2", "twitter": "1F4D2" },
            "shortcode": "ledger",
            "category": "object"
        },
        {
            "name": "page_with_curl",
            "unicode": { "apple": "1F4C3", "google": "1F4C3", "twitter": "1F4C3" },
            "shortcode": "page_with_curl",
            "category": "object"
        },
        {
            "name": "scroll",
            "unicode": { "apple": "1F4DC", "google": "1F4DC", "twitter": "1F4DC" },
            "shortcode": "scroll",
            "category": "object"
        },
        {
            "name": "page_facing_up",
            "unicode": { "apple": "1F4C4", "google": "1F4C4", "twitter": "1F4C4" },
            "shortcode": "page_facing_up",
            "category": "object"
        },
        {
            "name": "newspaper",
            "unicode": { "apple": "1F4F0", "google": "1F4F0", "twitter": "1F4F0" },
            "shortcode": "newspaper",
            "category": "object"
        },
        {
            "name": "rolled_up_newspaper",
            "unicode": { "apple": "1F5DE-FE0F", "google": "1F5DE-FE0F", "twitter": "1F5DE-FE0F" },
            "shortcode": "rolled_up_newspaper",
            "category": "object"
        },
        {
            "name": "bookmark_tabs",
            "unicode": { "apple": "1F4D1", "google": "1F4D1", "twitter": "1F4D1" },
            "shortcode": "bookmark_tabs",
            "category": "object"
        },
        {
            "name": "bookmark",
            "unicode": { "apple": "1F516", "google": "1F516", "twitter": "1F516" },
            "shortcode": "bookmark",
            "category": "object"
        },
        {
            "name": "label",
            "unicode": { "apple": "1F3F7-FE0F", "google": "1F3F7-FE0F", "twitter": "1F3F7-FE0F" },
            "shortcode": "label",
            "category": "object"
        },
        {
            "name": "moneybag",
            "unicode": { "apple": "1F4B0", "google": "1F4B0", "twitter": "1F4B0" },
            "shortcode": "moneybag",
            "category": "object"
        },
        {
            "name": "yen",
            "unicode": { "apple": "1F4B4", "google": "1F4B4", "twitter": "1F4B4" },
            "shortcode": "yen",
            "category": "object"
        },
        {
            "name": "dollar",
            "unicode": { "apple": "1F4B5", "google": "1F4B5", "twitter": "1F4B5" },
            "shortcode": "dollar",
            "category": "object"
        },
        {
            "name": "euro",
            "unicode": { "apple": "1F4B6", "google": "1F4B6", "twitter": "1F4B6" },
            "shortcode": "euro",
            "category": "object"
        },
        {
            "name": "pound",
            "unicode": { "apple": "1F4B7", "google": "1F4B7", "twitter": "1F4B7" },
            "shortcode": "pound",
            "category": "object"
        },
        {
            "name": "money_with_wings",
            "unicode": { "apple": "1F4B8", "google": "1F4B8", "twitter": "1F4B8" },
            "shortcode": "money_with_wings",
            "category": "object"
        },
        {
            "name": "credit_card",
            "unicode": { "apple": "1F4B3", "google": "1F4B3", "twitter": "1F4B3" },
            "shortcode": "credit_card",
            "category": "object"
        },
        {
            "name": "chart",
            "unicode": { "apple": "1F4B9", "google": "1F4B9", "twitter": "1F4B9" },
            "shortcode": "chart",
            "category": "object"
        },
        {
            "name": "currency_exchange",
            "unicode": { "apple": "1F4B1", "google": "1F4B1", "twitter": "1F4B1" },
            "shortcode": "currency_exchange",
            "category": "object"
        },
        {
            "name": "heavy_dollar_sign",
            "unicode": { "apple": "1F4B2", "google": "1F4B2", "twitter": "1F4B2" },
            "shortcode": "heavy_dollar_sign",
            "category": "object"
        },
        {
            "name": "email",
            "unicode": { "apple": "2709-FE0F", "google": "2709-FE0F", "twitter": "2709-FE0F" },
            "shortcode": "email",
            "category": "object"
        },
        {
            "name": "e-mail",
            "unicode": { "apple": "1F4E7", "google": "1F4E7", "twitter": "1F4E7" },
            "shortcode": "e-mail",
            "category": "object"
        },
        {
            "name": "incoming_envelope",
            "unicode": { "apple": "1F4E8", "google": "1F4E8", "twitter": "1F4E8" },
            "shortcode": "incoming_envelope",
            "category": "object"
        },
        {
            "name": "envelope_with_arrow",
            "unicode": { "apple": "1F4E9", "google": "1F4E9", "twitter": "1F4E9" },
            "shortcode": "envelope_with_arrow",
            "category": "object"
        },
        {
            "name": "outbox_tray",
            "unicode": { "apple": "1F4E4", "google": "1F4E4", "twitter": "1F4E4" },
            "shortcode": "outbox_tray",
            "category": "object"
        },
        {
            "name": "inbox_tray",
            "unicode": { "apple": "1F4E5", "google": "1F4E5", "twitter": "1F4E5" },
            "shortcode": "inbox_tray",
            "category": "object"
        },
        {
            "name": "package",
            "unicode": { "apple": "1F4E6", "google": "1F4E6", "twitter": "1F4E6" },
            "shortcode": "package",
            "category": "object"
        },
        {
            "name": "mailbox",
            "unicode": { "apple": "1F4EB", "google": "1F4EB", "twitter": "1F4EB" },
            "shortcode": "mailbox",
            "category": "object"
        },
        {
            "name": "mailbox_closed",
            "unicode": { "apple": "1F4EA", "google": "1F4EA", "twitter": "1F4EA" },
            "shortcode": "mailbox_closed",
            "category": "object"
        },
        {
            "name": "mailbox_with_mail",
            "unicode": { "apple": "1F4EC", "google": "1F4EC", "twitter": "1F4EC" },
            "shortcode": "mailbox_with_mail",
            "category": "object"
        },
        {
            "name": "mailbox_with_no_mail",
            "unicode": { "apple": "1F4ED", "google": "1F4ED", "twitter": "1F4ED" },
            "shortcode": "mailbox_with_no_mail",
            "category": "object"
        },
        {
            "name": "postbox",
            "unicode": { "apple": "1F4EE", "google": "1F4EE", "twitter": "1F4EE" },
            "shortcode": "postbox",
            "category": "object"
        },
        {
            "name": "ballot_box_with_ballot",
            "unicode": { "apple": "1F5F3-FE0F", "google": "1F5F3-FE0F", "twitter": "1F5F3-FE0F" },
            "shortcode": "ballot_box_with_ballot",
            "category": "object"
        },
        {
            "name": "pencil2",
            "unicode": { "apple": "270F-FE0F", "google": "270F-FE0F", "twitter": "270F-FE0F" },
            "shortcode": "pencil2",
            "category": "object"
        },
        {
            "name": "black_nib",
            "unicode": { "apple": "2712-FE0F", "google": "2712-FE0F", "twitter": "2712-FE0F" },
            "shortcode": "black_nib",
            "category": "object"
        },
        {
            "name": "lower_left_fountain_pen",
            "unicode": { "apple": "1F58B-FE0F", "google": "1F58B-FE0F", "twitter": "1F58B-FE0F" },
            "shortcode": "lower_left_fountain_pen",
            "category": "object"
        },
        {
            "name": "lower_left_ballpoint_pen",
            "unicode": { "apple": "1F58A-FE0F", "google": "1F58A-FE0F", "twitter": "1F58A-FE0F" },
            "shortcode": "lower_left_ballpoint_pen",
            "category": "object"
        },
        {
            "name": "lower_left_paintbrush",
            "unicode": { "apple": "1F58C-FE0F", "google": "1F58C-FE0F", "twitter": "1F58C-FE0F" },
            "shortcode": "lower_left_paintbrush",
            "category": "object"
        },
        {
            "name": "lower_left_crayon",
            "unicode": { "apple": "1F58D-FE0F", "google": "1F58D-FE0F", "twitter": "1F58D-FE0F" },
            "shortcode": "lower_left_crayon",
            "category": "object"
        },
        {
            "name": "memo",
            "unicode": { "apple": "1F4DD", "google": "1F4DD", "twitter": "1F4DD" },
            "shortcode": "memo",
            "category": "object"
        },
        {
            "name": "briefcase",
            "unicode": { "apple": "1F4BC", "google": "1F4BC", "twitter": "1F4BC" },
            "shortcode": "briefcase",
            "category": "object"
        },
        {
            "name": "file_folder",
            "unicode": { "apple": "1F4C1", "google": "1F4C1", "twitter": "1F4C1" },
            "shortcode": "file_folder",
            "category": "object"
        },
        {
            "name": "open_file_folder",
            "unicode": { "apple": "1F4C2", "google": "1F4C2", "twitter": "1F4C2" },
            "shortcode": "open_file_folder",
            "category": "object"
        },
        {
            "name": "card_index_dividers",
            "unicode": { "apple": "1F5C2-FE0F", "google": "1F5C2-FE0F", "twitter": "1F5C2-FE0F" },
            "shortcode": "card_index_dividers",
            "category": "object"
        },
        {
            "name": "date",
            "unicode": { "apple": "1F4C5", "google": "1F4C5", "twitter": "1F4C5" },
            "shortcode": "date",
            "category": "object"
        },
        {
            "name": "calendar",
            "unicode": { "apple": "1F4C6", "google": "1F4C6", "twitter": "1F4C6" },
            "shortcode": "calendar",
            "category": "object"
        },
        {
            "name": "spiral_note_pad",
            "unicode": { "apple": "1F5D2-FE0F", "google": "1F5D2-FE0F", "twitter": "1F5D2-FE0F" },
            "shortcode": "spiral_note_pad",
            "category": "object"
        },
        {
            "name": "spiral_calendar_pad",
            "unicode": { "apple": "1F5D3-FE0F", "google": "1F5D3-FE0F", "twitter": "1F5D3-FE0F" },
            "shortcode": "spiral_calendar_pad",
            "category": "object"
        },
        {
            "name": "card_index",
            "unicode": { "apple": "1F4C7", "google": "1F4C7", "twitter": "1F4C7" },
            "shortcode": "card_index",
            "category": "object"
        },
        {
            "name": "chart_with_upwards_trend",
            "unicode": { "apple": "1F4C8", "google": "1F4C8", "twitter": "1F4C8" },
            "shortcode": "chart_with_upwards_trend",
            "category": "object"
        },
        {
            "name": "chart_with_downwards_trend",
            "unicode": { "apple": "1F4C9", "google": "1F4C9", "twitter": "1F4C9" },
            "shortcode": "chart_with_downwards_trend",
            "category": "object"
        },
        {
            "name": "bar_chart",
            "unicode": { "apple": "1F4CA", "google": "1F4CA", "twitter": "1F4CA" },
            "shortcode": "bar_chart",
            "category": "object"
        },
        {
            "name": "clipboard",
            "unicode": { "apple": "1F4CB", "google": "1F4CB", "twitter": "1F4CB" },
            "shortcode": "clipboard",
            "category": "object"
        },
        {
            "name": "pushpin",
            "unicode": { "apple": "1F4CC", "google": "1F4CC", "twitter": "1F4CC" },
            "shortcode": "pushpin",
            "category": "object"
        },
        {
            "name": "round_pushpin",
            "unicode": { "apple": "1F4CD", "google": "1F4CD", "twitter": "1F4CD" },
            "shortcode": "round_pushpin",
            "category": "object"
        },
        {
            "name": "paperclip",
            "unicode": { "apple": "1F4CE", "google": "1F4CE", "twitter": "1F4CE" },
            "shortcode": "paperclip",
            "category": "object"
        },
        {
            "name": "linked_paperclips",
            "unicode": { "apple": "1F587-FE0F", "google": "1F587-FE0F", "twitter": "1F587-FE0F" },
            "shortcode": "linked_paperclips",
            "category": "object"
        },
        {
            "name": "straight_ruler",
            "unicode": { "apple": "1F4CF", "google": "1F4CF", "twitter": "1F4CF" },
            "shortcode": "straight_ruler",
            "category": "object"
        },
        {
            "name": "triangular_ruler",
            "unicode": { "apple": "1F4D0", "google": "1F4D0", "twitter": "1F4D0" },
            "shortcode": "triangular_ruler",
            "category": "object"
        },
        {
            "name": "scissors",
            "unicode": { "apple": "2702-FE0F", "google": "2702-FE0F", "twitter": "2702-FE0F" },
            "shortcode": "scissors",
            "category": "object"
        },
        {
            "name": "card_file_box",
            "unicode": { "apple": "1F5C3-FE0F", "google": "1F5C3-FE0F", "twitter": "1F5C3-FE0F" },
            "shortcode": "card_file_box",
            "category": "object"
        },
        {
            "name": "file_cabinet",
            "unicode": { "apple": "1F5C4-FE0F", "google": "1F5C4-FE0F", "twitter": "1F5C4-FE0F" },
            "shortcode": "file_cabinet",
            "category": "object"
        },
        {
            "name": "wastebasket",
            "unicode": { "apple": "1F5D1-FE0F", "google": "1F5D1-FE0F", "twitter": "1F5D1-FE0F" },
            "shortcode": "wastebasket",
            "category": "object"
        },
        {
            "name": "lock",
            "unicode": { "apple": "1F512", "google": "1F512", "twitter": "1F512" },
            "shortcode": "lock",
            "category": "object"
        },
        {
            "name": "unlock",
            "unicode": { "apple": "1F513", "google": "1F513", "twitter": "1F513" },
            "shortcode": "unlock",
            "category": "object"
        },
        {
            "name": "lock_with_ink_pen",
            "unicode": { "apple": "1F50F", "google": "1F50F", "twitter": "1F50F" },
            "shortcode": "lock_with_ink_pen",
            "category": "object"
        },
        {
            "name": "closed_lock_with_key",
            "unicode": { "apple": "1F510", "google": "1F510", "twitter": "1F510" },
            "shortcode": "closed_lock_with_key",
            "category": "object"
        },
        {
            "name": "key",
            "unicode": { "apple": "1F511", "google": "1F511", "twitter": "1F511" },
            "shortcode": "key",
            "category": "object"
        },
        {
            "name": "old_key",
            "unicode": { "apple": "1F5DD-FE0F", "google": "1F5DD-FE0F", "twitter": "1F5DD-FE0F" },
            "shortcode": "old_key",
            "category": "object"
        },
        {
            "name": "hammer",
            "unicode": { "apple": "1F528", "google": "1F528", "twitter": "1F528" },
            "shortcode": "hammer",
            "category": "object"
        },
        {
            "name": "pick",
            "unicode": { "apple": "26CF-FE0F", "google": "26CF-FE0F", "twitter": "26CF-FE0F" },
            "shortcode": "pick",
            "category": "object"
        },
        {
            "name": "hammer_and_pick",
            "unicode": { "apple": "2692-FE0F", "google": "2692-FE0F", "twitter": "2692-FE0F" },
            "shortcode": "hammer_and_pick",
            "category": "object"
        },
        {
            "name": "hammer_and_wrench",
            "unicode": { "apple": "1F6E0-FE0F", "google": "1F6E0-FE0F", "twitter": "1F6E0-FE0F" },
            "shortcode": "hammer_and_wrench",
            "category": "object"
        },
        {
            "name": "dagger_knife",
            "unicode": { "apple": "1F5E1-FE0F", "google": "1F5E1-FE0F", "twitter": "1F5E1-FE0F" },
            "shortcode": "dagger_knife",
            "category": "object"
        },
        {
            "name": "crossed_swords",
            "unicode": { "apple": "2694-FE0F", "google": "2694-FE0F", "twitter": "2694-FE0F" },
            "shortcode": "crossed_swords",
            "category": "object"
        },
        {
            "name": "gun",
            "unicode": { "apple": "1F52B", "google": "1F52B", "twitter": "1F52B" },
            "shortcode": "gun",
            "category": "object"
        },
        {
            "name": "bow_and_arrow",
            "unicode": { "apple": "1F3F9", "google": "1F3F9", "twitter": "1F3F9" },
            "shortcode": "bow_and_arrow",
            "category": "object"
        },
        {
            "name": "shield",
            "unicode": { "apple": "1F6E1-FE0F", "google": "1F6E1-FE0F", "twitter": "1F6E1-FE0F" },
            "shortcode": "shield",
            "category": "object"
        },
        {
            "name": "wrench",
            "unicode": { "apple": "1F527", "google": "1F527", "twitter": "1F527" },
            "shortcode": "wrench",
            "category": "object"
        },
        {
            "name": "nut_and_bolt",
            "unicode": { "apple": "1F529", "google": "1F529", "twitter": "1F529" },
            "shortcode": "nut_and_bolt",
            "category": "object"
        },
        {
            "name": "gear",
            "unicode": { "apple": "2699-FE0F", "google": "2699-FE0F", "twitter": "2699-FE0F" },
            "shortcode": "gear",
            "category": "object"
        },
        {
            "name": "compression",
            "unicode": { "apple": "1F5DC-FE0F", "google": "1F5DC-FE0F", "twitter": "1F5DC-FE0F" },
            "shortcode": "compression",
            "category": "object"
        },
        {
            "name": "alembic",
            "unicode": { "apple": "2697-FE0F", "google": "2697-FE0F", "twitter": "2697-FE0F" },
            "shortcode": "alembic",
            "category": "object"
        },
        {
            "name": "scales",
            "unicode": { "apple": "2696-FE0F", "google": "2696-FE0F", "twitter": "2696-FE0F" },
            "shortcode": "scales",
            "category": "object"
        },
        {
            "name": "link",
            "unicode": { "apple": "1F517", "google": "1F517", "twitter": "1F517" },
            "shortcode": "link",
            "category": "object"
        },
        {
            "name": "chains",
            "unicode": { "apple": "26D3-FE0F", "google": "26D3-FE0F", "twitter": "26D3-FE0F" },
            "shortcode": "chains",
            "category": "object"
        },
        {
            "name": "syringe",
            "unicode": { "apple": "1F489", "google": "1F489", "twitter": "1F489" },
            "shortcode": "syringe",
            "category": "object"
        },
        {
            "name": "pill",
            "unicode": { "apple": "1F48A", "google": "1F48A", "twitter": "1F48A" },
            "shortcode": "pill",
            "category": "object"
        },
        {
            "name": "smoking",
            "unicode": { "apple": "1F6AC", "google": "1F6AC", "twitter": "1F6AC" },
            "shortcode": "smoking",
            "category": "object"
        },
        {
            "name": "coffin",
            "unicode": { "apple": "26B0-FE0F", "google": "26B0-FE0F", "twitter": "26B0-FE0F" },
            "shortcode": "coffin",
            "category": "object"
        },
        {
            "name": "funeral_urn",
            "unicode": { "apple": "26B1-FE0F", "google": "26B1-FE0F", "twitter": "26B1-FE0F" },
            "shortcode": "funeral_urn",
            "category": "object"
        },
        {
            "name": "moyai",
            "unicode": { "apple": "1F5FF", "google": "1F5FF", "twitter": "1F5FF" },
            "shortcode": "moyai",
            "category": "object"
        },
        {
            "name": "oil_drum",
            "unicode": { "apple": "1F6E2-FE0F", "google": "1F6E2-FE0F", "twitter": "1F6E2-FE0F" },
            "shortcode": "oil_drum",
            "category": "object"
        },
        {
            "name": "crystal_ball",
            "unicode": { "apple": "1F52E", "google": "1F52E", "twitter": "1F52E" },
            "shortcode": "crystal_ball",
            "category": "object"
        },
        {
            "name": "shopping_trolley",
            "unicode": { "apple": "1F6D2", "google": "1F6D2", "twitter": "1F6D2" },
            "shortcode": "shopping_trolley",
            "category": "object"
        },
        {
            "name": "atm",
            "unicode": { "apple": "1F3E7", "google": "1F3E7", "twitter": "1F3E7" },
            "shortcode": "atm",
            "category": "symbol"
        },
        {
            "name": "put_litter_in_its_place",
            "unicode": { "apple": "1F6AE", "google": "1F6AE", "twitter": "1F6AE" },
            "shortcode": "put_litter_in_its_place",
            "category": "symbol"
        },
        {
            "name": "potable_water",
            "unicode": { "apple": "1F6B0", "google": "1F6B0", "twitter": "1F6B0" },
            "shortcode": "potable_water",
            "category": "symbol"
        },
        {
            "name": "wheelchair",
            "unicode": { "apple": "267F", "google": "267F", "twitter": "267F" },
            "shortcode": "wheelchair",
            "category": "symbol"
        },
        {
            "name": "mens",
            "unicode": { "apple": "1F6B9", "google": "1F6B9", "twitter": "1F6B9" },
            "shortcode": "mens",
            "category": "symbol"
        },
        {
            "name": "womens",
            "unicode": { "apple": "1F6BA", "google": "1F6BA", "twitter": "1F6BA" },
            "shortcode": "womens",
            "category": "symbol"
        },
        {
            "name": "restroom",
            "unicode": { "apple": "1F6BB", "google": "1F6BB", "twitter": "1F6BB" },
            "shortcode": "restroom",
            "category": "symbol"
        },
        {
            "name": "baby_symbol",
            "unicode": { "apple": "1F6BC", "google": "1F6BC", "twitter": "1F6BC" },
            "shortcode": "baby_symbol",
            "category": "symbol"
        },
        {
            "name": "wc",
            "unicode": { "apple": "1F6BE", "google": "1F6BE", "twitter": "1F6BE" },
            "shortcode": "wc",
            "category": "symbol"
        },
        {
            "name": "passport_control",
            "unicode": { "apple": "1F6C2", "google": "1F6C2", "twitter": "1F6C2" },
            "shortcode": "passport_control",
            "category": "symbol"
        },
        {
            "name": "customs",
            "unicode": { "apple": "1F6C3", "google": "1F6C3", "twitter": "1F6C3" },
            "shortcode": "customs",
            "category": "symbol"
        },
        {
            "name": "baggage_claim",
            "unicode": { "apple": "1F6C4", "google": "1F6C4", "twitter": "1F6C4" },
            "shortcode": "baggage_claim",
            "category": "symbol"
        },
        {
            "name": "left_luggage",
            "unicode": { "apple": "1F6C5", "google": "1F6C5", "twitter": "1F6C5" },
            "shortcode": "left_luggage",
            "category": "symbol"
        },
        {
            "name": "warning",
            "unicode": { "apple": "26A0-FE0F", "google": "26A0-FE0F", "twitter": "26A0-FE0F" },
            "shortcode": "warning",
            "category": "symbol"
        },
        {
            "name": "children_crossing",
            "unicode": { "apple": "1F6B8", "google": "1F6B8", "twitter": "1F6B8" },
            "shortcode": "children_crossing",
            "category": "symbol"
        },
        {
            "name": "no_entry",
            "unicode": { "apple": "26D4", "google": "26D4", "twitter": "26D4" },
            "shortcode": "no_entry",
            "category": "symbol"
        },
        {
            "name": "no_entry_sign",
            "unicode": { "apple": "1F6AB", "google": "1F6AB", "twitter": "1F6AB" },
            "shortcode": "no_entry_sign",
            "category": "symbol"
        },
        {
            "name": "no_bicycles",
            "unicode": { "apple": "1F6B3", "google": "1F6B3", "twitter": "1F6B3" },
            "shortcode": "no_bicycles",
            "category": "symbol"
        },
        {
            "name": "no_smoking",
            "unicode": { "apple": "1F6AD", "google": "1F6AD", "twitter": "1F6AD" },
            "shortcode": "no_smoking",
            "category": "symbol"
        },
        {
            "name": "do_not_litter",
            "unicode": { "apple": "1F6AF", "google": "1F6AF", "twitter": "1F6AF" },
            "shortcode": "do_not_litter",
            "category": "symbol"
        },
        {
            "name": "non-potable_water",
            "unicode": { "apple": "1F6B1", "google": "1F6B1", "twitter": "1F6B1" },
            "shortcode": "non-potable_water",
            "category": "symbol"
        },
        {
            "name": "no_pedestrians",
            "unicode": { "apple": "1F6B7", "google": "1F6B7", "twitter": "1F6B7" },
            "shortcode": "no_pedestrians",
            "category": "symbol"
        },
        {
            "name": "no_mobile_phones",
            "unicode": { "apple": "1F4F5", "google": "1F4F5", "twitter": "1F4F5" },
            "shortcode": "no_mobile_phones",
            "category": "symbol"
        },
        {
            "name": "underage",
            "unicode": { "apple": "1F51E", "google": "1F51E", "twitter": "1F51E" },
            "shortcode": "underage",
            "category": "symbol"
        },
        {
            "name": "radioactive_sign",
            "unicode": { "apple": "2622-FE0F", "google": "2622-FE0F", "twitter": "2622-FE0F" },
            "shortcode": "radioactive_sign",
            "category": "symbol"
        },
        {
            "name": "biohazard_sign",
            "unicode": { "apple": "2623-FE0F", "google": "2623-FE0F", "twitter": "2623-FE0F" },
            "shortcode": "biohazard_sign",
            "category": "symbol"
        },
        {
            "name": "arrow_up",
            "unicode": { "apple": "2B06-FE0F", "google": "2B06-FE0F", "twitter": "2B06-FE0F" },
            "shortcode": "arrow_up",
            "category": "symbol"
        },
        {
            "name": "arrow_upper_right",
            "unicode": { "apple": "2197-FE0F", "google": "2197-FE0F", "twitter": "2197-FE0F" },
            "shortcode": "arrow_upper_right",
            "category": "symbol"
        },
        {
            "name": "arrow_right",
            "unicode": { "apple": "27A1-FE0F", "google": "27A1-FE0F", "twitter": "27A1-FE0F" },
            "shortcode": "arrow_right",
            "category": "symbol"
        },
        {
            "name": "arrow_lower_right",
            "unicode": { "apple": "2198-FE0F", "google": "2198-FE0F", "twitter": "2198-FE0F" },
            "shortcode": "arrow_lower_right",
            "category": "symbol"
        },
        {
            "name": "arrow_down",
            "unicode": { "apple": "2B07-FE0F", "google": "2B07-FE0F", "twitter": "2B07-FE0F" },
            "shortcode": "arrow_down",
            "category": "symbol"
        },
        {
            "name": "arrow_lower_left",
            "unicode": { "apple": "2199-FE0F", "google": "2199-FE0F", "twitter": "2199-FE0F" },
            "shortcode": "arrow_lower_left",
            "category": "symbol"
        },
        {
            "name": "arrow_left",
            "unicode": { "apple": "2B05-FE0F", "google": "2B05-FE0F", "twitter": "2B05-FE0F" },
            "shortcode": "arrow_left",
            "category": "symbol"
        },
        {
            "name": "arrow_upper_left",
            "unicode": { "apple": "2196-FE0F", "google": "2196-FE0F", "twitter": "2196-FE0F" },
            "shortcode": "arrow_upper_left",
            "category": "symbol"
        },
        {
            "name": "arrow_up_down",
            "unicode": { "apple": "2195-FE0F", "google": "2195-FE0F", "twitter": "2195-FE0F" },
            "shortcode": "arrow_up_down",
            "category": "symbol"
        },
        {
            "name": "left_right_arrow",
            "unicode": { "apple": "2194-FE0F", "google": "2194-FE0F", "twitter": "2194-FE0F" },
            "shortcode": "left_right_arrow",
            "category": "symbol"
        },
        {
            "name": "leftwards_arrow_with_hook",
            "unicode": { "apple": "21A9-FE0F", "google": "21A9-FE0F", "twitter": "21A9-FE0F" },
            "shortcode": "leftwards_arrow_with_hook",
            "category": "symbol"
        },
        {
            "name": "arrow_right_hook",
            "unicode": { "apple": "21AA-FE0F", "google": "21AA-FE0F", "twitter": "21AA-FE0F" },
            "shortcode": "arrow_right_hook",
            "category": "symbol"
        },
        {
            "name": "arrow_heading_up",
            "unicode": { "apple": "2934-FE0F", "google": "2934-FE0F", "twitter": "2934-FE0F" },
            "shortcode": "arrow_heading_up",
            "category": "symbol"
        },
        {
            "name": "arrow_heading_down",
            "unicode": { "apple": "2935-FE0F", "google": "2935-FE0F", "twitter": "2935-FE0F" },
            "shortcode": "arrow_heading_down",
            "category": "symbol"
        },
        {
            "name": "arrows_clockwise",
            "unicode": { "apple": "1F503", "google": "1F503", "twitter": "1F503" },
            "shortcode": "arrows_clockwise",
            "category": "symbol"
        },
        {
            "name": "arrows_counterclockwise",
            "unicode": { "apple": "1F504", "google": "1F504", "twitter": "1F504" },
            "shortcode": "arrows_counterclockwise",
            "category": "symbol"
        },
        {
            "name": "back",
            "unicode": { "apple": "1F519", "google": "1F519", "twitter": "1F519" },
            "shortcode": "back",
            "category": "symbol"
        },
        {
            "name": "end",
            "unicode": { "apple": "1F51A", "google": "1F51A", "twitter": "1F51A" },
            "shortcode": "end",
            "category": "symbol"
        },
        {
            "name": "on",
            "unicode": { "apple": "1F51B", "google": "1F51B", "twitter": "1F51B" },
            "shortcode": "on",
            "category": "symbol"
        },
        {
            "name": "soon",
            "unicode": { "apple": "1F51C", "google": "1F51C", "twitter": "1F51C" },
            "shortcode": "soon",
            "category": "symbol"
        },
        {
            "name": "top",
            "unicode": { "apple": "1F51D", "google": "1F51D", "twitter": "1F51D" },
            "shortcode": "top",
            "category": "symbol"
        },
        {
            "name": "place_of_worship",
            "unicode": { "apple": "1F6D0", "google": "1F6D0", "twitter": "1F6D0" },
            "shortcode": "place_of_worship",
            "category": "symbol"
        },
        {
            "name": "atom_symbol",
            "unicode": { "apple": "269B-FE0F", "google": "269B-FE0F", "twitter": "269B-FE0F" },
            "shortcode": "atom_symbol",
            "category": "symbol"
        },
        {
            "name": "om_symbol",
            "unicode": { "apple": "1F549-FE0F", "google": "1F549-FE0F", "twitter": "1F549-FE0F" },
            "shortcode": "om_symbol",
            "category": "symbol"
        },
        {
            "name": "star_of_david",
            "unicode": { "apple": "2721-FE0F", "google": "2721-FE0F", "twitter": "2721-FE0F" },
            "shortcode": "star_of_david",
            "category": "symbol"
        },
        {
            "name": "wheel_of_dharma",
            "unicode": { "apple": "2638-FE0F", "google": "2638-FE0F", "twitter": "2638-FE0F" },
            "shortcode": "wheel_of_dharma",
            "category": "symbol"
        },
        {
            "name": "yin_yang",
            "unicode": { "apple": "262F-FE0F", "google": "262F-FE0F", "twitter": "262F-FE0F" },
            "shortcode": "yin_yang",
            "category": "symbol"
        },
        {
            "name": "latin_cross",
            "unicode": { "apple": "271D-FE0F", "google": "271D-FE0F", "twitter": "271D-FE0F" },
            "shortcode": "latin_cross",
            "category": "symbol"
        },
        {
            "name": "orthodox_cross",
            "unicode": { "apple": "2626-FE0F", "google": "2626-FE0F", "twitter": "2626-FE0F" },
            "shortcode": "orthodox_cross",
            "category": "symbol"
        },
        {
            "name": "star_and_crescent",
            "unicode": { "apple": "262A-FE0F", "google": "262A-FE0F", "twitter": "262A-FE0F" },
            "shortcode": "star_and_crescent",
            "category": "symbol"
        },
        {
            "name": "peace_symbol",
            "unicode": { "apple": "262E-FE0F", "google": "262E-FE0F", "twitter": "262E-FE0F" },
            "shortcode": "peace_symbol",
            "category": "symbol"
        },
        {
            "name": "menorah_with_nine_branches",
            "unicode": { "apple": "1F54E", "google": "1F54E", "twitter": "1F54E" },
            "shortcode": "menorah_with_nine_branches",
            "category": "symbol"
        },
        {
            "name": "six_pointed_star",
            "unicode": { "apple": "1F52F", "google": "1F52F", "twitter": "1F52F" },
            "shortcode": "six_pointed_star",
            "category": "symbol"
        },
        {
            "name": "aries",
            "unicode": { "apple": "2648", "google": "2648", "twitter": "2648" },
            "shortcode": "aries",
            "category": "symbol"
        },
        {
            "name": "taurus",
            "unicode": { "apple": "2649", "google": "2649", "twitter": "2649" },
            "shortcode": "taurus",
            "category": "symbol"
        },
        {
            "name": "gemini",
            "unicode": { "apple": "264A", "google": "264A", "twitter": "264A" },
            "shortcode": "gemini",
            "category": "symbol"
        },
        {
            "name": "cancer",
            "unicode": { "apple": "264B", "google": "264B", "twitter": "264B" },
            "shortcode": "cancer",
            "category": "symbol"
        },
        {
            "name": "leo",
            "unicode": { "apple": "264C", "google": "264C", "twitter": "264C" },
            "shortcode": "leo",
            "category": "symbol"
        },
        {
            "name": "virgo",
            "unicode": { "apple": "264D", "google": "264D", "twitter": "264D" },
            "shortcode": "virgo",
            "category": "symbol"
        },
        {
            "name": "libra",
            "unicode": { "apple": "264E", "google": "264E", "twitter": "264E" },
            "shortcode": "libra",
            "category": "symbol"
        },
        {
            "name": "scorpius",
            "unicode": { "apple": "264F", "google": "264F", "twitter": "264F" },
            "shortcode": "scorpius",
            "category": "symbol"
        },
        {
            "name": "sagittarius",
            "unicode": { "apple": "2650", "google": "2650", "twitter": "2650" },
            "shortcode": "sagittarius",
            "category": "symbol"
        },
        {
            "name": "capricorn",
            "unicode": { "apple": "2651", "google": "2651", "twitter": "2651" },
            "shortcode": "capricorn",
            "category": "symbol"
        },
        {
            "name": "aquarius",
            "unicode": { "apple": "2652", "google": "2652", "twitter": "2652" },
            "shortcode": "aquarius",
            "category": "symbol"
        },
        {
            "name": "pisces",
            "unicode": { "apple": "2653", "google": "2653", "twitter": "2653" },
            "shortcode": "pisces",
            "category": "symbol"
        },
        {
            "name": "ophiuchus",
            "unicode": { "apple": "26CE", "google": "26CE", "twitter": "26CE" },
            "shortcode": "ophiuchus",
            "category": "symbol"
        },
        {
            "name": "twisted_rightwards_arrows",
            "unicode": { "apple": "1F500", "google": "1F500", "twitter": "1F500" },
            "shortcode": "twisted_rightwards_arrows",
            "category": "symbol"
        },
        {
            "name": "repeat",
            "unicode": { "apple": "1F501", "google": "1F501", "twitter": "1F501" },
            "shortcode": "repeat",
            "category": "symbol"
        },
        {
            "name": "repeat_one",
            "unicode": { "apple": "1F502", "google": "1F502", "twitter": "1F502" },
            "shortcode": "repeat_one",
            "category": "symbol"
        },
        {
            "name": "arrow_forward",
            "unicode": { "apple": "25B6-FE0F", "google": "25B6-FE0F", "twitter": "25B6-FE0F" },
            "shortcode": "arrow_forward",
            "category": "symbol"
        },
        {
            "name": "fast_forward",
            "unicode": { "apple": "23E9", "google": "23E9", "twitter": "23E9" },
            "shortcode": "fast_forward",
            "category": "symbol"
        },
        {
            "name": "black_right_pointing_double_triangle_with_vertical_bar",
            "unicode": { "apple": "23ED-FE0F", "google": "23ED-FE0F", "twitter": "23ED-FE0F" },
            "shortcode": "black_right_pointing_double_triangle_with_vertical_bar",
            "category": "symbol"
        },
        {
            "name": "black_right_pointing_triangle_with_double_vertical_bar",
            "unicode": { "apple": "23EF-FE0F", "google": "23EF-FE0F", "twitter": "23EF-FE0F" },
            "shortcode": "black_right_pointing_triangle_with_double_vertical_bar",
            "category": "symbol"
        },
        {
            "name": "arrow_backward",
            "unicode": { "apple": "25C0-FE0F", "google": "25C0-FE0F", "twitter": "25C0-FE0F" },
            "shortcode": "arrow_backward",
            "category": "symbol"
        },
        {
            "name": "rewind",
            "unicode": { "apple": "23EA", "google": "23EA", "twitter": "23EA" },
            "shortcode": "rewind",
            "category": "symbol"
        },
        {
            "name": "black_left_pointing_double_triangle_with_vertical_bar",
            "unicode": { "apple": "23EE-FE0F", "google": "23EE-FE0F", "twitter": "23EE-FE0F" },
            "shortcode": "black_left_pointing_double_triangle_with_vertical_bar",
            "category": "symbol"
        },
        {
            "name": "arrow_up_small",
            "unicode": { "apple": "1F53C", "google": "1F53C", "twitter": "1F53C" },
            "shortcode": "arrow_up_small",
            "category": "symbol"
        },
        {
            "name": "arrow_double_up",
            "unicode": { "apple": "23EB", "google": "23EB", "twitter": "23EB" },
            "shortcode": "arrow_double_up",
            "category": "symbol"
        },
        {
            "name": "arrow_down_small",
            "unicode": { "apple": "1F53D", "google": "1F53D", "twitter": "1F53D" },
            "shortcode": "arrow_down_small",
            "category": "symbol"
        },
        {
            "name": "arrow_double_down",
            "unicode": { "apple": "23EC", "google": "23EC", "twitter": "23EC" },
            "shortcode": "arrow_double_down",
            "category": "symbol"
        },
        {
            "name": "double_vertical_bar",
            "unicode": { "apple": "23F8-FE0F", "google": "23F8-FE0F", "twitter": "23F8-FE0F" },
            "shortcode": "double_vertical_bar",
            "category": "symbol"
        },
        {
            "name": "black_square_for_stop",
            "unicode": { "apple": "23F9-FE0F", "google": "23F9-FE0F", "twitter": "23F9-FE0F" },
            "shortcode": "black_square_for_stop",
            "category": "symbol"
        },
        {
            "name": "black_circle_for_record",
            "unicode": { "apple": "23FA-FE0F", "google": "23FA-FE0F", "twitter": "23FA-FE0F" },
            "shortcode": "black_circle_for_record",
            "category": "symbol"
        },
        {
            "name": "eject",
            "unicode": { "apple": "23CF-FE0F", "google": "23CF-FE0F", "twitter": "23CF-FE0F" },
            "shortcode": "eject",
            "category": "symbol"
        },
        {
            "name": "cinema",
            "unicode": { "apple": "1F3A6", "google": "1F3A6", "twitter": "1F3A6" },
            "shortcode": "cinema",
            "category": "symbol"
        },
        {
            "name": "low_brightness",
            "unicode": { "apple": "1F505", "google": "1F505", "twitter": "1F505" },
            "shortcode": "low_brightness",
            "category": "symbol"
        },
        {
            "name": "high_brightness",
            "unicode": { "apple": "1F506", "google": "1F506", "twitter": "1F506" },
            "shortcode": "high_brightness",
            "category": "symbol"
        },
        {
            "name": "signal_strength",
            "unicode": { "apple": "1F4F6", "google": "1F4F6", "twitter": "1F4F6" },
            "shortcode": "signal_strength",
            "category": "symbol"
        },
        {
            "name": "vibration_mode",
            "unicode": { "apple": "1F4F3", "google": "1F4F3", "twitter": "1F4F3" },
            "shortcode": "vibration_mode",
            "category": "symbol"
        },
        {
            "name": "mobile_phone_off",
            "unicode": { "apple": "1F4F4", "google": "1F4F4", "twitter": "1F4F4" },
            "shortcode": "mobile_phone_off",
            "category": "symbol"
        },
        {
            "name": "recycle",
            "unicode": { "apple": "267B-FE0F", "google": "267B-FE0F", "twitter": "267B-FE0F" },
            "shortcode": "recycle",
            "category": "symbol"
        },
        {
            "name": "fleur_de_lis",
            "unicode": { "apple": "269C-FE0F", "google": "269C-FE0F", "twitter": "269C-FE0F" },
            "shortcode": "fleur_de_lis",
            "category": "symbol"
        },
        {
            "name": "trident",
            "unicode": { "apple": "1F531", "google": "1F531", "twitter": "1F531" },
            "shortcode": "trident",
            "category": "symbol"
        },
        {
            "name": "name_badge",
            "unicode": { "apple": "1F4DB", "google": "1F4DB", "twitter": "1F4DB" },
            "shortcode": "name_badge",
            "category": "symbol"
        },
        {
            "name": "beginner",
            "unicode": { "apple": "1F530", "google": "1F530", "twitter": "1F530" },
            "shortcode": "beginner",
            "category": "symbol"
        },
        {
            "name": "o",
            "unicode": { "apple": "2B55", "google": "2B55", "twitter": "2B55" },
            "shortcode": "o",
            "category": "symbol"
        },
        {
            "name": "white_check_mark",
            "unicode": { "apple": "2705", "google": "2705", "twitter": "2705" },
            "shortcode": "white_check_mark",
            "category": "symbol"
        },
        {
            "name": "ballot_box_with_check",
            "unicode": { "apple": "2611-FE0F", "google": "2611-FE0F", "twitter": "2611-FE0F" },
            "shortcode": "ballot_box_with_check",
            "category": "symbol"
        },
        {
            "name": "heavy_check_mark",
            "unicode": { "apple": "2714-FE0F", "google": "2714-FE0F", "twitter": "2714-FE0F" },
            "shortcode": "heavy_check_mark",
            "category": "symbol"
        },
        {
            "name": "heavy_multiplication_x",
            "unicode": { "apple": "2716-FE0F", "google": "2716-FE0F", "twitter": "2716-FE0F" },
            "shortcode": "heavy_multiplication_x",
            "category": "symbol"
        },
        {
            "name": "x",
            "unicode": { "apple": "274C", "google": "274C", "twitter": "274C" },
            "shortcode": "x",
            "category": "symbol"
        },
        {
            "name": "negative_squared_cross_mark",
            "unicode": { "apple": "274E", "google": "274E", "twitter": "274E" },
            "shortcode": "negative_squared_cross_mark",
            "category": "symbol"
        },
        {
            "name": "heavy_plus_sign",
            "unicode": { "apple": "2795", "google": "2795", "twitter": "2795" },
            "shortcode": "heavy_plus_sign",
            "category": "symbol"
        },
        {
            "name": "heavy_minus_sign",
            "unicode": { "apple": "2796", "google": "2796", "twitter": "2796" },
            "shortcode": "heavy_minus_sign",
            "category": "symbol"
        },
        {
            "name": "heavy_division_sign",
            "unicode": { "apple": "2797", "google": "2797", "twitter": "2797" },
            "shortcode": "heavy_division_sign",
            "category": "symbol"
        },
        {
            "name": "curly_loop",
            "unicode": { "apple": "27B0", "google": "27B0", "twitter": "27B0" },
            "shortcode": "curly_loop",
            "category": "symbol"
        },
        {
            "name": "loop",
            "unicode": { "apple": "27BF", "google": "27BF", "twitter": "27BF" },
            "shortcode": "loop",
            "category": "symbol"
        },
        {
            "name": "part_alternation_mark",
            "unicode": { "apple": "303D-FE0F", "google": "303D-FE0F", "twitter": "303D-FE0F" },
            "shortcode": "part_alternation_mark",
            "category": "symbol"
        },
        {
            "name": "eight_spoked_asterisk",
            "unicode": { "apple": "2733-FE0F", "google": "2733-FE0F", "twitter": "2733-FE0F" },
            "shortcode": "eight_spoked_asterisk",
            "category": "symbol"
        },
        {
            "name": "eight_pointed_black_star",
            "unicode": { "apple": "2734-FE0F", "google": "2734-FE0F", "twitter": "2734-FE0F" },
            "shortcode": "eight_pointed_black_star",
            "category": "symbol"
        },
        {
            "name": "sparkle",
            "unicode": { "apple": "2747-FE0F", "google": "2747-FE0F", "twitter": "2747-FE0F" },
            "shortcode": "sparkle",
            "category": "symbol"
        },
        {
            "name": "bangbang",
            "unicode": { "apple": "203C-FE0F", "google": "203C-FE0F", "twitter": "203C-FE0F" },
            "shortcode": "bangbang",
            "category": "symbol"
        },
        {
            "name": "interrobang",
            "unicode": { "apple": "2049-FE0F", "google": "2049-FE0F", "twitter": "2049-FE0F" },
            "shortcode": "interrobang",
            "category": "symbol"
        },
        {
            "name": "question",
            "unicode": { "apple": "2753", "google": "2753", "twitter": "2753" },
            "shortcode": "question",
            "category": "symbol"
        },
        {
            "name": "grey_question",
            "unicode": { "apple": "2754", "google": "2754", "twitter": "2754" },
            "shortcode": "grey_question",
            "category": "symbol"
        },
        {
            "name": "grey_exclamation",
            "unicode": { "apple": "2755", "google": "2755", "twitter": "2755" },
            "shortcode": "grey_exclamation",
            "category": "symbol"
        },
        {
            "name": "exclamation",
            "unicode": { "apple": "2757", "google": "2757", "twitter": "2757" },
            "shortcode": "exclamation",
            "category": "symbol"
        },
        {
            "name": "wavy_dash",
            "unicode": { "apple": "3030-FE0F", "google": "3030-FE0F", "twitter": "3030-FE0F" },
            "shortcode": "wavy_dash",
            "category": "symbol"
        },
        {
            "name": "copyright",
            "unicode": { "apple": "00A9-FE0F", "google": "00A9-FE0F", "twitter": "00A9-FE0F" },
            "shortcode": "copyright",
            "category": "symbol"
        },
        {
            "name": "registered",
            "unicode": { "apple": "00AE-FE0F", "google": "00AE-FE0F", "twitter": "00AE-FE0F" },
            "shortcode": "registered",
            "category": "symbol"
        },
        {
            "name": "tm",
            "unicode": { "apple": "2122-FE0F", "google": "2122-FE0F", "twitter": "2122-FE0F" },
            "shortcode": "tm",
            "category": "symbol"
        },
        {
            "name": "hash",
            "unicode": { "apple": "0023-FE0F-20E3", "google": "0023-FE0F-20E3", "twitter": "0023-FE0F-20E3" },
            "shortcode": "hash",
            "category": "symbol"
        },
        {
            "name": "keycap_star",
            "unicode": { "apple": "002A-FE0F-20E3", "google": "002A-FE0F-20E3", "twitter": "002A-FE0F-20E3" },
            "shortcode": "keycap_star",
            "category": "symbol"
        },
        {
            "name": "zero",
            "unicode": { "apple": "0030-FE0F-20E3", "google": "0030-FE0F-20E3", "twitter": "0030-FE0F-20E3" },
            "shortcode": "zero",
            "category": "symbol"
        },
        {
            "name": "one",
            "unicode": { "apple": "0031-FE0F-20E3", "google": "0031-FE0F-20E3", "twitter": "0031-FE0F-20E3" },
            "shortcode": "one",
            "category": "symbol"
        },
        {
            "name": "two",
            "unicode": { "apple": "0032-FE0F-20E3", "google": "0032-FE0F-20E3", "twitter": "0032-FE0F-20E3" },
            "shortcode": "two",
            "category": "symbol"
        },
        {
            "name": "three",
            "unicode": { "apple": "0033-FE0F-20E3", "google": "0033-FE0F-20E3", "twitter": "0033-FE0F-20E3" },
            "shortcode": "three",
            "category": "symbol"
        },
        {
            "name": "four",
            "unicode": { "apple": "0034-FE0F-20E3", "google": "0034-FE0F-20E3", "twitter": "0034-FE0F-20E3" },
            "shortcode": "four",
            "category": "symbol"
        },
        {
            "name": "five",
            "unicode": { "apple": "0035-FE0F-20E3", "google": "0035-FE0F-20E3", "twitter": "0035-FE0F-20E3" },
            "shortcode": "five",
            "category": "symbol"
        },
        {
            "name": "six",
            "unicode": { "apple": "0036-FE0F-20E3", "google": "0036-FE0F-20E3", "twitter": "0036-FE0F-20E3" },
            "shortcode": "six",
            "category": "symbol"
        },
        {
            "name": "seven",
            "unicode": { "apple": "0037-FE0F-20E3", "google": "0037-FE0F-20E3", "twitter": "0037-FE0F-20E3" },
            "shortcode": "seven",
            "category": "symbol"
        },
        {
            "name": "eight",
            "unicode": { "apple": "0038-FE0F-20E3", "google": "0038-FE0F-20E3", "twitter": "0038-FE0F-20E3" },
            "shortcode": "eight",
            "category": "symbol"
        },
        {
            "name": "nine",
            "unicode": { "apple": "0039-FE0F-20E3", "google": "0039-FE0F-20E3", "twitter": "0039-FE0F-20E3" },
            "shortcode": "nine",
            "category": "symbol"
        },
        {
            "name": "keycap_ten",
            "unicode": { "apple": "1F51F", "google": "1F51F", "twitter": "1F51F" },
            "shortcode": "keycap_ten",
            "category": "symbol"
        },
        {
            "name": "100",
            "unicode": { "apple": "1F4AF", "google": "1F4AF", "twitter": "1F4AF" },
            "shortcode": "100",
            "category": "symbol"
        },
        {
            "name": "capital_abcd",
            "unicode": { "apple": "1F520", "google": "1F520", "twitter": "1F520" },
            "shortcode": "capital_abcd",
            "category": "symbol"
        },
        {
            "name": "abcd",
            "unicode": { "apple": "1F521", "google": "1F521", "twitter": "1F521" },
            "shortcode": "abcd",
            "category": "symbol"
        },
        {
            "name": "1234",
            "unicode": { "apple": "1F522", "google": "1F522", "twitter": "1F522" },
            "shortcode": "1234",
            "category": "symbol"
        },
        {
            "name": "symbols",
            "unicode": { "apple": "1F523", "google": "1F523", "twitter": "1F523" },
            "shortcode": "symbols",
            "category": "symbol"
        },
        {
            "name": "abc",
            "unicode": { "apple": "1F524", "google": "1F524", "twitter": "1F524" },
            "shortcode": "abc",
            "category": "symbol"
        },
        {
            "name": "a",
            "unicode": { "apple": "1F170-FE0F", "google": "1F170-FE0F", "twitter": "1F170-FE0F" },
            "shortcode": "a",
            "category": "symbol"
        },
        {
            "name": "ab",
            "unicode": { "apple": "1F18E", "google": "1F18E", "twitter": "1F18E" },
            "shortcode": "ab",
            "category": "symbol"
        },
        {
            "name": "b",
            "unicode": { "apple": "1F171-FE0F", "google": "1F171-FE0F", "twitter": "1F171-FE0F" },
            "shortcode": "b",
            "category": "symbol"
        },
        {
            "name": "cl",
            "unicode": { "apple": "1F191", "google": "1F191", "twitter": "1F191" },
            "shortcode": "cl",
            "category": "symbol"
        },
        {
            "name": "cool",
            "unicode": { "apple": "1F192", "google": "1F192", "twitter": "1F192" },
            "shortcode": "cool",
            "category": "symbol"
        },
        {
            "name": "free",
            "unicode": { "apple": "1F193", "google": "1F193", "twitter": "1F193" },
            "shortcode": "free",
            "category": "symbol"
        },
        {
            "name": "information_source",
            "unicode": { "apple": "2139-FE0F", "google": "2139-FE0F", "twitter": "2139-FE0F" },
            "shortcode": "information_source",
            "category": "symbol"
        },
        {
            "name": "id",
            "unicode": { "apple": "1F194", "google": "1F194", "twitter": "1F194" },
            "shortcode": "id",
            "category": "symbol"
        },
        {
            "name": "m",
            "unicode": { "apple": "24C2-FE0F", "google": "24C2-FE0F", "twitter": "24C2-FE0F" },
            "shortcode": "m",
            "category": "symbol"
        },
        {
            "name": "new",
            "unicode": { "apple": "1F195", "google": "1F195", "twitter": "1F195" },
            "shortcode": "new",
            "category": "symbol"
        },
        {
            "name": "ng",
            "unicode": { "apple": "1F196", "google": "1F196", "twitter": "1F196" },
            "shortcode": "ng",
            "category": "symbol"
        },
        {
            "name": "o2",
            "unicode": { "apple": "1F17E-FE0F", "google": "1F17E-FE0F", "twitter": "1F17E-FE0F" },
            "shortcode": "o2",
            "category": "symbol"
        },
        {
            "name": "ok",
            "unicode": { "apple": "1F197", "google": "1F197", "twitter": "1F197" },
            "shortcode": "ok",
            "category": "symbol"
        },
        {
            "name": "parking",
            "unicode": { "apple": "1F17F-FE0F", "google": "1F17F-FE0F", "twitter": "1F17F-FE0F" },
            "shortcode": "parking",
            "category": "symbol"
        },
        {
            "name": "sos",
            "unicode": { "apple": "1F198", "google": "1F198", "twitter": "1F198" },
            "shortcode": "sos",
            "category": "symbol"
        },
        {
            "name": "up",
            "unicode": { "apple": "1F199", "google": "1F199", "twitter": "1F199" },
            "shortcode": "up",
            "category": "symbol"
        },
        {
            "name": "vs",
            "unicode": { "apple": "1F19A", "google": "1F19A", "twitter": "1F19A" },
            "shortcode": "vs",
            "category": "symbol"
        },
        {
            "name": "koko",
            "unicode": { "apple": "1F201", "google": "1F201", "twitter": "1F201" },
            "shortcode": "koko",
            "category": "symbol"
        },
        {
            "name": "sa",
            "unicode": { "apple": "1F202-FE0F", "google": "1F202-FE0F", "twitter": "1F202-FE0F" },
            "shortcode": "sa",
            "category": "symbol"
        },
        {
            "name": "u6708",
            "unicode": { "apple": "1F237-FE0F", "google": "1F237-FE0F", "twitter": "1F237-FE0F" },
            "shortcode": "u6708",
            "category": "symbol"
        },
        {
            "name": "u6709",
            "unicode": { "apple": "1F236", "google": "1F236", "twitter": "1F236" },
            "shortcode": "u6709",
            "category": "symbol"
        },
        {
            "name": "u6307",
            "unicode": { "apple": "1F22F", "google": "1F22F", "twitter": "1F22F" },
            "shortcode": "u6307",
            "category": "symbol"
        },
        {
            "name": "ideograph_advantage",
            "unicode": { "apple": "1F250", "google": "1F250", "twitter": "1F250" },
            "shortcode": "ideograph_advantage",
            "category": "symbol"
        },
        {
            "name": "u5272",
            "unicode": { "apple": "1F239", "google": "1F239", "twitter": "1F239" },
            "shortcode": "u5272",
            "category": "symbol"
        },
        {
            "name": "u7121",
            "unicode": { "apple": "1F21A", "google": "1F21A", "twitter": "1F21A" },
            "shortcode": "u7121",
            "category": "symbol"
        },
        {
            "name": "u7981",
            "unicode": { "apple": "1F232", "google": "1F232", "twitter": "1F232" },
            "shortcode": "u7981",
            "category": "symbol"
        },
        {
            "name": "accept",
            "unicode": { "apple": "1F251", "google": "1F251", "twitter": "1F251" },
            "shortcode": "accept",
            "category": "symbol"
        },
        {
            "name": "u7533",
            "unicode": { "apple": "1F238", "google": "1F238", "twitter": "1F238" },
            "shortcode": "u7533",
            "category": "symbol"
        },
        {
            "name": "u5408",
            "unicode": { "apple": "1F234", "google": "1F234", "twitter": "1F234" },
            "shortcode": "u5408",
            "category": "symbol"
        },
        {
            "name": "u7a7a",
            "unicode": { "apple": "1F233", "google": "1F233", "twitter": "1F233" },
            "shortcode": "u7a7a",
            "category": "symbol"
        },
        {
            "name": "congratulations",
            "unicode": { "apple": "3297-FE0F", "google": "3297-FE0F", "twitter": "3297-FE0F" },
            "shortcode": "congratulations",
            "category": "symbol"
        },
        {
            "name": "secret",
            "unicode": { "apple": "3299-FE0F", "google": "3299-FE0F", "twitter": "3299-FE0F" },
            "shortcode": "secret",
            "category": "symbol"
        },
        {
            "name": "u55b6",
            "unicode": { "apple": "1F23A", "google": "1F23A", "twitter": "1F23A" },
            "shortcode": "u55b6",
            "category": "symbol"
        },
        {
            "name": "u6e80",
            "unicode": { "apple": "1F235", "google": "1F235", "twitter": "1F235" },
            "shortcode": "u6e80",
            "category": "symbol"
        },
        {
            "name": "black_small_square",
            "unicode": { "apple": "25AA-FE0F", "google": "25AA-FE0F", "twitter": "25AA-FE0F" },
            "shortcode": "black_small_square",
            "category": "symbol"
        },
        {
            "name": "white_small_square",
            "unicode": { "apple": "25AB-FE0F", "google": "25AB-FE0F", "twitter": "25AB-FE0F" },
            "shortcode": "white_small_square",
            "category": "symbol"
        },
        {
            "name": "white_medium_square",
            "unicode": { "apple": "25FB-FE0F", "google": "25FB-FE0F", "twitter": "25FB-FE0F" },
            "shortcode": "white_medium_square",
            "category": "symbol"
        },
        {
            "name": "black_medium_square",
            "unicode": { "apple": "25FC-FE0F", "google": "25FC-FE0F", "twitter": "25FC-FE0F" },
            "shortcode": "black_medium_square",
            "category": "symbol"
        },
        {
            "name": "white_medium_small_square",
            "unicode": { "apple": "25FD", "google": "25FD", "twitter": "25FD" },
            "shortcode": "white_medium_small_square",
            "category": "symbol"
        },
        {
            "name": "black_medium_small_square",
            "unicode": { "apple": "25FE", "google": "25FE", "twitter": "25FE" },
            "shortcode": "black_medium_small_square",
            "category": "symbol"
        },
        {
            "name": "black_large_square",
            "unicode": { "apple": "2B1B", "google": "2B1B", "twitter": "2B1B" },
            "shortcode": "black_large_square",
            "category": "symbol"
        },
        {
            "name": "white_large_square",
            "unicode": { "apple": "2B1C", "google": "2B1C", "twitter": "2B1C" },
            "shortcode": "white_large_square",
            "category": "symbol"
        },
        {
            "name": "large_orange_diamond",
            "unicode": { "apple": "1F536", "google": "1F536", "twitter": "1F536" },
            "shortcode": "large_orange_diamond",
            "category": "symbol"
        },
        {
            "name": "large_blue_diamond",
            "unicode": { "apple": "1F537", "google": "1F537", "twitter": "1F537" },
            "shortcode": "large_blue_diamond",
            "category": "symbol"
        },
        {
            "name": "small_orange_diamond",
            "unicode": { "apple": "1F538", "google": "1F538", "twitter": "1F538" },
            "shortcode": "small_orange_diamond",
            "category": "symbol"
        },
        {
            "name": "small_blue_diamond",
            "unicode": { "apple": "1F539", "google": "1F539", "twitter": "1F539" },
            "shortcode": "small_blue_diamond",
            "category": "symbol"
        },
        {
            "name": "small_red_triangle",
            "unicode": { "apple": "1F53A", "google": "1F53A", "twitter": "1F53A" },
            "shortcode": "small_red_triangle",
            "category": "symbol"
        },
        {
            "name": "small_red_triangle_down",
            "unicode": { "apple": "1F53B", "google": "1F53B", "twitter": "1F53B" },
            "shortcode": "small_red_triangle_down",
            "category": "symbol"
        },
        {
            "name": "diamond_shape_with_a_dot_inside",
            "unicode": { "apple": "1F4A0", "google": "1F4A0", "twitter": "1F4A0" },
            "shortcode": "diamond_shape_with_a_dot_inside",
            "category": "symbol"
        },
        {
            "name": "radio_button",
            "unicode": { "apple": "1F518", "google": "1F518", "twitter": "1F518" },
            "shortcode": "radio_button",
            "category": "symbol"
        },
        {
            "name": "black_square_button",
            "unicode": { "apple": "1F532", "google": "1F532", "twitter": "1F532" },
            "shortcode": "black_square_button",
            "category": "symbol"
        },
        {
            "name": "white_square_button",
            "unicode": { "apple": "1F533", "google": "1F533", "twitter": "1F533" },
            "shortcode": "white_square_button",
            "category": "symbol"
        },
        {
            "name": "white_circle",
            "unicode": { "apple": "26AA", "google": "26AA", "twitter": "26AA" },
            "shortcode": "white_circle",
            "category": "symbol"
        },
        {
            "name": "black_circle",
            "unicode": { "apple": "26AB", "google": "26AB", "twitter": "26AB" },
            "shortcode": "black_circle",
            "category": "symbol"
        },
        {
            "name": "red_circle",
            "unicode": { "apple": "1F534", "google": "1F534", "twitter": "1F534" },
            "shortcode": "red_circle",
            "category": "symbol"
        },
        {
            "name": "large_blue_circle",
            "unicode": { "apple": "1F535", "google": "1F535", "twitter": "1F535" },
            "shortcode": "large_blue_circle",
            "category": "symbol"
        },
        {
            "name": "checkered_flag",
            "unicode": { "apple": "1F3C1", "google": "1F3C1", "twitter": "1F3C1" },
            "shortcode": "checkered_flag",
            "category": "flag"
        },
        {
            "name": "triangular_flag_on_post",
            "unicode": { "apple": "1F6A9", "google": "1F6A9", "twitter": "1F6A9" },
            "shortcode": "triangular_flag_on_post",
            "category": "flag"
        },
        {
            "name": "crossed_flags",
            "unicode": { "apple": "1F38C", "google": "1F38C", "twitter": "1F38C" },
            "shortcode": "crossed_flags",
            "category": "flag"
        },
        {
            "name": "waving_black_flag",
            "unicode": { "apple": "1F3F4", "google": "1F3F4", "twitter": "1F3F4" },
            "shortcode": "waving_black_flag",
            "category": "flag"
        },
        {
            "name": "waving_white_flag",
            "unicode": { "apple": "1F3F3-FE0F", "google": "1F3F3-FE0F", "twitter": "1F3F3-FE0F" },
            "shortcode": "waving_white_flag",
            "category": "flag"
        },
        {
            "name": "rainbow-flag",
            "unicode": {
                "apple": "1F3F3-FE0F-200D-1F308",
                "google": "1F3F3-FE0F-200D-1F308",
                "twitter": "1F3F3-FE0F-200D-1F308"
            },
            "shortcode": "rainbow-flag",
            "category": "flag"
        },
        {
            "name": "flag-ac",
            "unicode": { "apple": "1F1E6-1F1E8", "google": "1F1E6-1F1E8", "twitter": "1F1E6-1F1E8" },
            "shortcode": "flag-ac",
            "category": "flag"
        },
        {
            "name": "flag-ad",
            "unicode": { "apple": "1F1E6-1F1E9", "google": "1F1E6-1F1E9", "twitter": "1F1E6-1F1E9" },
            "shortcode": "flag-ad",
            "category": "flag"
        },
        {
            "name": "flag-ae",
            "unicode": { "apple": "1F1E6-1F1EA", "google": "1F1E6-1F1EA", "twitter": "1F1E6-1F1EA" },
            "shortcode": "flag-ae",
            "category": "flag"
        },
        {
            "name": "flag-af",
            "unicode": { "apple": "1F1E6-1F1EB", "google": "1F1E6-1F1EB", "twitter": "1F1E6-1F1EB" },
            "shortcode": "flag-af",
            "category": "flag"
        },
        {
            "name": "flag-ag",
            "unicode": { "apple": "1F1E6-1F1EC", "google": "1F1E6-1F1EC", "twitter": "1F1E6-1F1EC" },
            "shortcode": "flag-ag",
            "category": "flag"
        },
        {
            "name": "flag-ai",
            "unicode": { "apple": "1F1E6-1F1EE", "google": "1F1E6-1F1EE", "twitter": "1F1E6-1F1EE" },
            "shortcode": "flag-ai",
            "category": "flag"
        },
        {
            "name": "flag-al",
            "unicode": { "apple": "1F1E6-1F1F1", "google": "1F1E6-1F1F1", "twitter": "1F1E6-1F1F1" },
            "shortcode": "flag-al",
            "category": "flag"
        },
        {
            "name": "flag-am",
            "unicode": { "apple": "1F1E6-1F1F2", "google": "1F1E6-1F1F2", "twitter": "1F1E6-1F1F2" },
            "shortcode": "flag-am",
            "category": "flag"
        },
        {
            "name": "flag-ao",
            "unicode": { "apple": "1F1E6-1F1F4", "google": "1F1E6-1F1F4", "twitter": "1F1E6-1F1F4" },
            "shortcode": "flag-ao",
            "category": "flag"
        },
        {
            "name": "flag-aq",
            "unicode": { "apple": "1F1E6-1F1F6", "google": "1F1E6-1F1F6", "twitter": "1F1E6-1F1F6" },
            "shortcode": "flag-aq",
            "category": "flag"
        },
        {
            "name": "flag-ar",
            "unicode": { "apple": "1F1E6-1F1F7", "google": "1F1E6-1F1F7", "twitter": "1F1E6-1F1F7" },
            "shortcode": "flag-ar",
            "category": "flag"
        },
        {
            "name": "flag-as",
            "unicode": { "apple": "1F1E6-1F1F8", "google": "1F1E6-1F1F8", "twitter": "1F1E6-1F1F8" },
            "shortcode": "flag-as",
            "category": "flag"
        },
        {
            "name": "flag-at",
            "unicode": { "apple": "1F1E6-1F1F9", "google": "1F1E6-1F1F9", "twitter": "1F1E6-1F1F9" },
            "shortcode": "flag-at",
            "category": "flag"
        },
        {
            "name": "flag-au",
            "unicode": { "apple": "1F1E6-1F1FA", "google": "1F1E6-1F1FA", "twitter": "1F1E6-1F1FA" },
            "shortcode": "flag-au",
            "category": "flag"
        },
        {
            "name": "flag-aw",
            "unicode": { "apple": "1F1E6-1F1FC", "google": "1F1E6-1F1FC", "twitter": "1F1E6-1F1FC" },
            "shortcode": "flag-aw",
            "category": "flag"
        },
        {
            "name": "flag-ax",
            "unicode": { "apple": "1F1E6-1F1FD", "google": "1F1E6-1F1FD", "twitter": "1F1E6-1F1FD" },
            "shortcode": "flag-ax",
            "category": "flag"
        },
        {
            "name": "flag-az",
            "unicode": { "apple": "1F1E6-1F1FF", "google": "1F1E6-1F1FF", "twitter": "1F1E6-1F1FF" },
            "shortcode": "flag-az",
            "category": "flag"
        },
        {
            "name": "flag-ba",
            "unicode": { "apple": "1F1E7-1F1E6", "google": "1F1E7-1F1E6", "twitter": "1F1E7-1F1E6" },
            "shortcode": "flag-ba",
            "category": "flag"
        },
        {
            "name": "flag-bb",
            "unicode": { "apple": "1F1E7-1F1E7", "google": "1F1E7-1F1E7", "twitter": "1F1E7-1F1E7" },
            "shortcode": "flag-bb",
            "category": "flag"
        },
        {
            "name": "flag-bd",
            "unicode": { "apple": "1F1E7-1F1E9", "google": "1F1E7-1F1E9", "twitter": "1F1E7-1F1E9" },
            "shortcode": "flag-bd",
            "category": "flag"
        },
        {
            "name": "flag-be",
            "unicode": { "apple": "1F1E7-1F1EA", "google": "1F1E7-1F1EA", "twitter": "1F1E7-1F1EA" },
            "shortcode": "flag-be",
            "category": "flag"
        },
        {
            "name": "flag-bf",
            "unicode": { "apple": "1F1E7-1F1EB", "google": "1F1E7-1F1EB", "twitter": "1F1E7-1F1EB" },
            "shortcode": "flag-bf",
            "category": "flag"
        },
        {
            "name": "flag-bg",
            "unicode": { "apple": "1F1E7-1F1EC", "google": "1F1E7-1F1EC", "twitter": "1F1E7-1F1EC" },
            "shortcode": "flag-bg",
            "category": "flag"
        },
        {
            "name": "flag-bh",
            "unicode": { "apple": "1F1E7-1F1ED", "google": "1F1E7-1F1ED", "twitter": "1F1E7-1F1ED" },
            "shortcode": "flag-bh",
            "category": "flag"
        },
        {
            "name": "flag-bi",
            "unicode": { "apple": "1F1E7-1F1EE", "google": "1F1E7-1F1EE", "twitter": "1F1E7-1F1EE" },
            "shortcode": "flag-bi",
            "category": "flag"
        },
        {
            "name": "flag-bj",
            "unicode": { "apple": "1F1E7-1F1EF", "google": "1F1E7-1F1EF", "twitter": "1F1E7-1F1EF" },
            "shortcode": "flag-bj",
            "category": "flag"
        },
        {
            "name": "flag-bm",
            "unicode": { "apple": "1F1E7-1F1F2", "google": "1F1E7-1F1F2", "twitter": "1F1E7-1F1F2" },
            "shortcode": "flag-bm",
            "category": "flag"
        },
        {
            "name": "flag-bn",
            "unicode": { "apple": "1F1E7-1F1F3", "google": "1F1E7-1F1F3", "twitter": "1F1E7-1F1F3" },
            "shortcode": "flag-bn",
            "category": "flag"
        },
        {
            "name": "flag-bo",
            "unicode": { "apple": "1F1E7-1F1F4", "google": "1F1E7-1F1F4", "twitter": "1F1E7-1F1F4" },
            "shortcode": "flag-bo",
            "category": "flag"
        },
        {
            "name": "flag-br",
            "unicode": { "apple": "1F1E7-1F1F7", "google": "1F1E7-1F1F7", "twitter": "1F1E7-1F1F7" },
            "shortcode": "flag-br",
            "category": "flag"
        },
        {
            "name": "flag-bs",
            "unicode": { "apple": "1F1E7-1F1F8", "google": "1F1E7-1F1F8", "twitter": "1F1E7-1F1F8" },
            "shortcode": "flag-bs",
            "category": "flag"
        },
        {
            "name": "flag-bt",
            "unicode": { "apple": "1F1E7-1F1F9", "google": "1F1E7-1F1F9", "twitter": "1F1E7-1F1F9" },
            "shortcode": "flag-bt",
            "category": "flag"
        },
        {
            "name": "flag-bv",
            "unicode": { "apple": "1F1E7-1F1FB", "google": "1F1E7-1F1FB", "twitter": "1F1E7-1F1FB" },
            "shortcode": "flag-bv",
            "category": "flag"
        },
        {
            "name": "flag-bw",
            "unicode": { "apple": "1F1E7-1F1FC", "google": "1F1E7-1F1FC", "twitter": "1F1E7-1F1FC" },
            "shortcode": "flag-bw",
            "category": "flag"
        },
        {
            "name": "flag-by",
            "unicode": { "apple": "1F1E7-1F1FE", "google": "1F1E7-1F1FE", "twitter": "1F1E7-1F1FE" },
            "shortcode": "flag-by",
            "category": "flag"
        },
        {
            "name": "flag-bz",
            "unicode": { "apple": "1F1E7-1F1FF", "google": "1F1E7-1F1FF", "twitter": "1F1E7-1F1FF" },
            "shortcode": "flag-bz",
            "category": "flag"
        },
        {
            "name": "flag-ca",
            "unicode": { "apple": "1F1E8-1F1E6", "google": "1F1E8-1F1E6", "twitter": "1F1E8-1F1E6" },
            "shortcode": "flag-ca",
            "category": "flag"
        },
        {
            "name": "flag-cc",
            "unicode": { "apple": "1F1E8-1F1E8", "google": "1F1E8-1F1E8", "twitter": "1F1E8-1F1E8" },
            "shortcode": "flag-cc",
            "category": "flag"
        },
        {
            "name": "flag-cd",
            "unicode": { "apple": "1F1E8-1F1E9", "google": "1F1E8-1F1E9", "twitter": "1F1E8-1F1E9" },
            "shortcode": "flag-cd",
            "category": "flag"
        },
        {
            "name": "flag-cf",
            "unicode": { "apple": "1F1E8-1F1EB", "google": "1F1E8-1F1EB", "twitter": "1F1E8-1F1EB" },
            "shortcode": "flag-cf",
            "category": "flag"
        },
        {
            "name": "flag-cg",
            "unicode": { "apple": "1F1E8-1F1EC", "google": "1F1E8-1F1EC", "twitter": "1F1E8-1F1EC" },
            "shortcode": "flag-cg",
            "category": "flag"
        },
        {
            "name": "flag-ch",
            "unicode": { "apple": "1F1E8-1F1ED", "google": "1F1E8-1F1ED", "twitter": "1F1E8-1F1ED" },
            "shortcode": "flag-ch",
            "category": "flag"
        },
        {
            "name": "flag-ci",
            "unicode": { "apple": "1F1E8-1F1EE", "google": "1F1E8-1F1EE", "twitter": "1F1E8-1F1EE" },
            "shortcode": "flag-ci",
            "category": "flag"
        },
        {
            "name": "flag-ck",
            "unicode": { "apple": "1F1E8-1F1F0", "google": "1F1E8-1F1F0", "twitter": "1F1E8-1F1F0" },
            "shortcode": "flag-ck",
            "category": "flag"
        },
        {
            "name": "flag-cl",
            "unicode": { "apple": "1F1E8-1F1F1", "google": "1F1E8-1F1F1", "twitter": "1F1E8-1F1F1" },
            "shortcode": "flag-cl",
            "category": "flag"
        },
        {
            "name": "flag-cm",
            "unicode": { "apple": "1F1E8-1F1F2", "google": "1F1E8-1F1F2", "twitter": "1F1E8-1F1F2" },
            "shortcode": "flag-cm",
            "category": "flag"
        },
        {
            "name": "cn",
            "unicode": { "apple": "1F1E8-1F1F3", "google": "1F1E8-1F1F3", "twitter": "1F1E8-1F1F3" },
            "shortcode": "cn",
            "category": "flag"
        },
        {
            "name": "flag-co",
            "unicode": { "apple": "1F1E8-1F1F4", "google": "1F1E8-1F1F4", "twitter": "1F1E8-1F1F4" },
            "shortcode": "flag-co",
            "category": "flag"
        },
        {
            "name": "flag-cp",
            "unicode": { "apple": "1F1E8-1F1F5", "google": "1F1E8-1F1F5", "twitter": "1F1E8-1F1F5" },
            "shortcode": "flag-cp",
            "category": "flag"
        },
        {
            "name": "flag-cr",
            "unicode": { "apple": "1F1E8-1F1F7", "google": "1F1E8-1F1F7", "twitter": "1F1E8-1F1F7" },
            "shortcode": "flag-cr",
            "category": "flag"
        },
        {
            "name": "flag-cu",
            "unicode": { "apple": "1F1E8-1F1FA", "google": "1F1E8-1F1FA", "twitter": "1F1E8-1F1FA" },
            "shortcode": "flag-cu",
            "category": "flag"
        },
        {
            "name": "flag-cv",
            "unicode": { "apple": "1F1E8-1F1FB", "google": "1F1E8-1F1FB", "twitter": "1F1E8-1F1FB" },
            "shortcode": "flag-cv",
            "category": "flag"
        },
        {
            "name": "flag-cw",
            "unicode": { "apple": "1F1E8-1F1FC", "google": "1F1E8-1F1FC", "twitter": "1F1E8-1F1FC" },
            "shortcode": "flag-cw",
            "category": "flag"
        },
        {
            "name": "flag-cx",
            "unicode": { "apple": "1F1E8-1F1FD", "google": "1F1E8-1F1FD", "twitter": "1F1E8-1F1FD" },
            "shortcode": "flag-cx",
            "category": "flag"
        },
        {
            "name": "flag-cy",
            "unicode": { "apple": "1F1E8-1F1FE", "google": "1F1E8-1F1FE", "twitter": "1F1E8-1F1FE" },
            "shortcode": "flag-cy",
            "category": "flag"
        },
        {
            "name": "flag-cz",
            "unicode": { "apple": "1F1E8-1F1FF", "google": "1F1E8-1F1FF", "twitter": "1F1E8-1F1FF" },
            "shortcode": "flag-cz",
            "category": "flag"
        },
        {
            "name": "de",
            "unicode": { "apple": "1F1E9-1F1EA", "google": "1F1E9-1F1EA", "twitter": "1F1E9-1F1EA" },
            "shortcode": "de",
            "category": "flag"
        },
        {
            "name": "flag-dj",
            "unicode": { "apple": "1F1E9-1F1EF", "google": "1F1E9-1F1EF", "twitter": "1F1E9-1F1EF" },
            "shortcode": "flag-dj",
            "category": "flag"
        },
        {
            "name": "flag-dk",
            "unicode": { "apple": "1F1E9-1F1F0", "google": "1F1E9-1F1F0", "twitter": "1F1E9-1F1F0" },
            "shortcode": "flag-dk",
            "category": "flag"
        },
        {
            "name": "flag-dm",
            "unicode": { "apple": "1F1E9-1F1F2", "google": "1F1E9-1F1F2", "twitter": "1F1E9-1F1F2" },
            "shortcode": "flag-dm",
            "category": "flag"
        },
        {
            "name": "flag-do",
            "unicode": { "apple": "1F1E9-1F1F4", "google": "1F1E9-1F1F4", "twitter": "1F1E9-1F1F4" },
            "shortcode": "flag-do",
            "category": "flag"
        },
        {
            "name": "flag-dz",
            "unicode": { "apple": "1F1E9-1F1FF", "google": "1F1E9-1F1FF", "twitter": "1F1E9-1F1FF" },
            "shortcode": "flag-dz",
            "category": "flag"
        },
        {
            "name": "flag-ec",
            "unicode": { "apple": "1F1EA-1F1E8", "google": "1F1EA-1F1E8", "twitter": "1F1EA-1F1E8" },
            "shortcode": "flag-ec",
            "category": "flag"
        },
        {
            "name": "flag-ee",
            "unicode": { "apple": "1F1EA-1F1EA", "google": "1F1EA-1F1EA", "twitter": "1F1EA-1F1EA" },
            "shortcode": "flag-ee",
            "category": "flag"
        },
        {
            "name": "flag-eg",
            "unicode": { "apple": "1F1EA-1F1EC", "google": "1F1EA-1F1EC", "twitter": "1F1EA-1F1EC" },
            "shortcode": "flag-eg",
            "category": "flag"
        },
        {
            "name": "flag-er",
            "unicode": { "apple": "1F1EA-1F1F7", "google": "1F1EA-1F1F7", "twitter": "1F1EA-1F1F7" },
            "shortcode": "flag-er",
            "category": "flag"
        },
        {
            "name": "es",
            "unicode": { "apple": "1F1EA-1F1F8", "google": "1F1EA-1F1F8", "twitter": "1F1EA-1F1F8" },
            "shortcode": "es",
            "category": "flag"
        },
        {
            "name": "flag-et",
            "unicode": { "apple": "1F1EA-1F1F9", "google": "1F1EA-1F1F9", "twitter": "1F1EA-1F1F9" },
            "shortcode": "flag-et",
            "category": "flag"
        },
        {
            "name": "flag-eu",
            "unicode": { "apple": "1F1EA-1F1FA", "google": "1F1EA-1F1FA", "twitter": "1F1EA-1F1FA" },
            "shortcode": "flag-eu",
            "category": "flag"
        },
        {
            "name": "flag-fi",
            "unicode": { "apple": "1F1EB-1F1EE", "google": "1F1EB-1F1EE", "twitter": "1F1EB-1F1EE" },
            "shortcode": "flag-fi",
            "category": "flag"
        },
        {
            "name": "flag-fj",
            "unicode": { "apple": "1F1EB-1F1EF", "google": "1F1EB-1F1EF", "twitter": "1F1EB-1F1EF" },
            "shortcode": "flag-fj",
            "category": "flag"
        },
        {
            "name": "flag-fm",
            "unicode": { "apple": "1F1EB-1F1F2", "google": "1F1EB-1F1F2", "twitter": "1F1EB-1F1F2" },
            "shortcode": "flag-fm",
            "category": "flag"
        },
        {
            "name": "flag-fo",
            "unicode": { "apple": "1F1EB-1F1F4", "google": "1F1EB-1F1F4", "twitter": "1F1EB-1F1F4" },
            "shortcode": "flag-fo",
            "category": "flag"
        },
        {
            "name": "fr",
            "unicode": { "apple": "1F1EB-1F1F7", "google": "1F1EB-1F1F7", "twitter": "1F1EB-1F1F7" },
            "shortcode": "fr",
            "category": "flag"
        },
        {
            "name": "flag-ga",
            "unicode": { "apple": "1F1EC-1F1E6", "google": "1F1EC-1F1E6", "twitter": "1F1EC-1F1E6" },
            "shortcode": "flag-ga",
            "category": "flag"
        },
        {
            "name": "gb",
            "unicode": { "apple": "1F1EC-1F1E7", "google": "1F1EC-1F1E7", "twitter": "1F1EC-1F1E7" },
            "shortcode": "gb",
            "category": "flag"
        },
        {
            "name": "flag-gd",
            "unicode": { "apple": "1F1EC-1F1E9", "google": "1F1EC-1F1E9", "twitter": "1F1EC-1F1E9" },
            "shortcode": "flag-gd",
            "category": "flag"
        },
        {
            "name": "flag-ge",
            "unicode": { "apple": "1F1EC-1F1EA", "google": "1F1EC-1F1EA", "twitter": "1F1EC-1F1EA" },
            "shortcode": "flag-ge",
            "category": "flag"
        },
        {
            "name": "flag-gg",
            "unicode": { "apple": "1F1EC-1F1EC", "google": "1F1EC-1F1EC", "twitter": "1F1EC-1F1EC" },
            "shortcode": "flag-gg",
            "category": "flag"
        },
        {
            "name": "flag-gh",
            "unicode": { "apple": "1F1EC-1F1ED", "google": "1F1EC-1F1ED", "twitter": "1F1EC-1F1ED" },
            "shortcode": "flag-gh",
            "category": "flag"
        },
        {
            "name": "flag-gi",
            "unicode": { "apple": "1F1EC-1F1EE", "google": "1F1EC-1F1EE", "twitter": "1F1EC-1F1EE" },
            "shortcode": "flag-gi",
            "category": "flag"
        },
        {
            "name": "flag-gl",
            "unicode": { "apple": "1F1EC-1F1F1", "google": "1F1EC-1F1F1", "twitter": "1F1EC-1F1F1" },
            "shortcode": "flag-gl",
            "category": "flag"
        },
        {
            "name": "flag-gm",
            "unicode": { "apple": "1F1EC-1F1F2", "google": "1F1EC-1F1F2", "twitter": "1F1EC-1F1F2" },
            "shortcode": "flag-gm",
            "category": "flag"
        },
        {
            "name": "flag-gn",
            "unicode": { "apple": "1F1EC-1F1F3", "google": "1F1EC-1F1F3", "twitter": "1F1EC-1F1F3" },
            "shortcode": "flag-gn",
            "category": "flag"
        },
        {
            "name": "flag-gq",
            "unicode": { "apple": "1F1EC-1F1F6", "google": "1F1EC-1F1F6", "twitter": "1F1EC-1F1F6" },
            "shortcode": "flag-gq",
            "category": "flag"
        },
        {
            "name": "flag-gr",
            "unicode": { "apple": "1F1EC-1F1F7", "google": "1F1EC-1F1F7", "twitter": "1F1EC-1F1F7" },
            "shortcode": "flag-gr",
            "category": "flag"
        },
        {
            "name": "flag-gt",
            "unicode": { "apple": "1F1EC-1F1F9", "google": "1F1EC-1F1F9", "twitter": "1F1EC-1F1F9" },
            "shortcode": "flag-gt",
            "category": "flag"
        },
        {
            "name": "flag-gu",
            "unicode": { "apple": "1F1EC-1F1FA", "google": "1F1EC-1F1FA", "twitter": "1F1EC-1F1FA" },
            "shortcode": "flag-gu",
            "category": "flag"
        },
        {
            "name": "flag-gw",
            "unicode": { "apple": "1F1EC-1F1FC", "google": "1F1EC-1F1FC", "twitter": "1F1EC-1F1FC" },
            "shortcode": "flag-gw",
            "category": "flag"
        },
        {
            "name": "flag-gy",
            "unicode": { "apple": "1F1EC-1F1FE", "google": "1F1EC-1F1FE", "twitter": "1F1EC-1F1FE" },
            "shortcode": "flag-gy",
            "category": "flag"
        },
        {
            "name": "flag-hk",
            "unicode": { "apple": "1F1ED-1F1F0", "google": "1F1ED-1F1F0", "twitter": "1F1ED-1F1F0" },
            "shortcode": "flag-hk",
            "category": "flag"
        },
        {
            "name": "flag-hm",
            "unicode": { "apple": "1F1ED-1F1F2", "google": "1F1ED-1F1F2", "twitter": "1F1ED-1F1F2" },
            "shortcode": "flag-hm",
            "category": "flag"
        },
        {
            "name": "flag-hn",
            "unicode": { "apple": "1F1ED-1F1F3", "google": "1F1ED-1F1F3", "twitter": "1F1ED-1F1F3" },
            "shortcode": "flag-hn",
            "category": "flag"
        },
        {
            "name": "flag-hr",
            "unicode": { "apple": "1F1ED-1F1F7", "google": "1F1ED-1F1F7", "twitter": "1F1ED-1F1F7" },
            "shortcode": "flag-hr",
            "category": "flag"
        },
        {
            "name": "flag-ht",
            "unicode": { "apple": "1F1ED-1F1F9", "google": "1F1ED-1F1F9", "twitter": "1F1ED-1F1F9" },
            "shortcode": "flag-ht",
            "category": "flag"
        },
        {
            "name": "flag-hu",
            "unicode": { "apple": "1F1ED-1F1FA", "google": "1F1ED-1F1FA", "twitter": "1F1ED-1F1FA" },
            "shortcode": "flag-hu",
            "category": "flag"
        },
        {
            "name": "flag-ic",
            "unicode": { "apple": "1F1EE-1F1E8", "google": "1F1EE-1F1E8", "twitter": "1F1EE-1F1E8" },
            "shortcode": "flag-ic",
            "category": "flag"
        },
        {
            "name": "flag-id",
            "unicode": { "apple": "1F1EE-1F1E9", "google": "1F1EE-1F1E9", "twitter": "1F1EE-1F1E9" },
            "shortcode": "flag-id",
            "category": "flag"
        },
        {
            "name": "flag-ie",
            "unicode": { "apple": "1F1EE-1F1EA", "google": "1F1EE-1F1EA", "twitter": "1F1EE-1F1EA" },
            "shortcode": "flag-ie",
            "category": "flag"
        },
        {
            "name": "flag-il",
            "unicode": { "apple": "1F1EE-1F1F1", "google": "1F1EE-1F1F1", "twitter": "1F1EE-1F1F1" },
            "shortcode": "flag-il",
            "category": "flag"
        },
        {
            "name": "flag-im",
            "unicode": { "apple": "1F1EE-1F1F2", "google": "1F1EE-1F1F2", "twitter": "1F1EE-1F1F2" },
            "shortcode": "flag-im",
            "category": "flag"
        },
        {
            "name": "flag-in",
            "unicode": { "apple": "1F1EE-1F1F3", "google": "1F1EE-1F1F3", "twitter": "1F1EE-1F1F3" },
            "shortcode": "flag-in",
            "category": "flag"
        },
        {
            "name": "flag-io",
            "unicode": { "apple": "1F1EE-1F1F4", "google": "1F1EE-1F1F4", "twitter": "1F1EE-1F1F4" },
            "shortcode": "flag-io",
            "category": "flag"
        },
        {
            "name": "flag-iq",
            "unicode": { "apple": "1F1EE-1F1F6", "google": "1F1EE-1F1F6", "twitter": "1F1EE-1F1F6" },
            "shortcode": "flag-iq",
            "category": "flag"
        },
        {
            "name": "flag-ir",
            "unicode": { "apple": "1F1EE-1F1F7", "google": "1F1EE-1F1F7", "twitter": "1F1EE-1F1F7" },
            "shortcode": "flag-ir",
            "category": "flag"
        },
        {
            "name": "flag-is",
            "unicode": { "apple": "1F1EE-1F1F8", "google": "1F1EE-1F1F8", "twitter": "1F1EE-1F1F8" },
            "shortcode": "flag-is",
            "category": "flag"
        },
        {
            "name": "it",
            "unicode": { "apple": "1F1EE-1F1F9", "google": "1F1EE-1F1F9", "twitter": "1F1EE-1F1F9" },
            "shortcode": "it",
            "category": "flag"
        },
        {
            "name": "flag-je",
            "unicode": { "apple": "1F1EF-1F1EA", "google": "1F1EF-1F1EA", "twitter": "1F1EF-1F1EA" },
            "shortcode": "flag-je",
            "category": "flag"
        },
        {
            "name": "flag-jm",
            "unicode": { "apple": "1F1EF-1F1F2", "google": "1F1EF-1F1F2", "twitter": "1F1EF-1F1F2" },
            "shortcode": "flag-jm",
            "category": "flag"
        },
        {
            "name": "flag-jo",
            "unicode": { "apple": "1F1EF-1F1F4", "google": "1F1EF-1F1F4", "twitter": "1F1EF-1F1F4" },
            "shortcode": "flag-jo",
            "category": "flag"
        },
        {
            "name": "jp",
            "unicode": { "apple": "1F1EF-1F1F5", "google": "1F1EF-1F1F5", "twitter": "1F1EF-1F1F5" },
            "shortcode": "jp",
            "category": "flag"
        },
        {
            "name": "flag-ke",
            "unicode": { "apple": "1F1F0-1F1EA", "google": "1F1F0-1F1EA", "twitter": "1F1F0-1F1EA" },
            "shortcode": "flag-ke",
            "category": "flag"
        },
        {
            "name": "flag-kg",
            "unicode": { "apple": "1F1F0-1F1EC", "google": "1F1F0-1F1EC", "twitter": "1F1F0-1F1EC" },
            "shortcode": "flag-kg",
            "category": "flag"
        },
        {
            "name": "flag-kh",
            "unicode": { "apple": "1F1F0-1F1ED", "google": "1F1F0-1F1ED", "twitter": "1F1F0-1F1ED" },
            "shortcode": "flag-kh",
            "category": "flag"
        },
        {
            "name": "flag-ki",
            "unicode": { "apple": "1F1F0-1F1EE", "google": "1F1F0-1F1EE", "twitter": "1F1F0-1F1EE" },
            "shortcode": "flag-ki",
            "category": "flag"
        },
        {
            "name": "flag-km",
            "unicode": { "apple": "1F1F0-1F1F2", "google": "1F1F0-1F1F2", "twitter": "1F1F0-1F1F2" },
            "shortcode": "flag-km",
            "category": "flag"
        },
        {
            "name": "flag-kn",
            "unicode": { "apple": "1F1F0-1F1F3", "google": "1F1F0-1F1F3", "twitter": "1F1F0-1F1F3" },
            "shortcode": "flag-kn",
            "category": "flag"
        },
        {
            "name": "flag-kp",
            "unicode": { "apple": "1F1F0-1F1F5", "google": "1F1F0-1F1F5", "twitter": "1F1F0-1F1F5" },
            "shortcode": "flag-kp",
            "category": "flag"
        },
        {
            "name": "kr",
            "unicode": { "apple": "1F1F0-1F1F7", "google": "1F1F0-1F1F7", "twitter": "1F1F0-1F1F7" },
            "shortcode": "kr",
            "category": "flag"
        },
        {
            "name": "flag-kw",
            "unicode": { "apple": "1F1F0-1F1FC", "google": "1F1F0-1F1FC", "twitter": "1F1F0-1F1FC" },
            "shortcode": "flag-kw",
            "category": "flag"
        },
        {
            "name": "flag-ky",
            "unicode": { "apple": "1F1F0-1F1FE", "google": "1F1F0-1F1FE", "twitter": "1F1F0-1F1FE" },
            "shortcode": "flag-ky",
            "category": "flag"
        },
        {
            "name": "flag-kz",
            "unicode": { "apple": "1F1F0-1F1FF", "google": "1F1F0-1F1FF", "twitter": "1F1F0-1F1FF" },
            "shortcode": "flag-kz",
            "category": "flag"
        },
        {
            "name": "flag-la",
            "unicode": { "apple": "1F1F1-1F1E6", "google": "1F1F1-1F1E6", "twitter": "1F1F1-1F1E6" },
            "shortcode": "flag-la",
            "category": "flag"
        },
        {
            "name": "flag-lb",
            "unicode": { "apple": "1F1F1-1F1E7", "google": "1F1F1-1F1E7", "twitter": "1F1F1-1F1E7" },
            "shortcode": "flag-lb",
            "category": "flag"
        },
        {
            "name": "flag-lc",
            "unicode": { "apple": "1F1F1-1F1E8", "google": "1F1F1-1F1E8", "twitter": "1F1F1-1F1E8" },
            "shortcode": "flag-lc",
            "category": "flag"
        },
        {
            "name": "flag-li",
            "unicode": { "apple": "1F1F1-1F1EE", "google": "1F1F1-1F1EE", "twitter": "1F1F1-1F1EE" },
            "shortcode": "flag-li",
            "category": "flag"
        },
        {
            "name": "flag-lk",
            "unicode": { "apple": "1F1F1-1F1F0", "google": "1F1F1-1F1F0", "twitter": "1F1F1-1F1F0" },
            "shortcode": "flag-lk",
            "category": "flag"
        },
        {
            "name": "flag-lr",
            "unicode": { "apple": "1F1F1-1F1F7", "google": "1F1F1-1F1F7", "twitter": "1F1F1-1F1F7" },
            "shortcode": "flag-lr",
            "category": "flag"
        },
        {
            "name": "flag-ls",
            "unicode": { "apple": "1F1F1-1F1F8", "google": "1F1F1-1F1F8", "twitter": "1F1F1-1F1F8" },
            "shortcode": "flag-ls",
            "category": "flag"
        },
        {
            "name": "flag-lt",
            "unicode": { "apple": "1F1F1-1F1F9", "google": "1F1F1-1F1F9", "twitter": "1F1F1-1F1F9" },
            "shortcode": "flag-lt",
            "category": "flag"
        },
        {
            "name": "flag-lu",
            "unicode": { "apple": "1F1F1-1F1FA", "google": "1F1F1-1F1FA", "twitter": "1F1F1-1F1FA" },
            "shortcode": "flag-lu",
            "category": "flag"
        },
        {
            "name": "flag-lv",
            "unicode": { "apple": "1F1F1-1F1FB", "google": "1F1F1-1F1FB", "twitter": "1F1F1-1F1FB" },
            "shortcode": "flag-lv",
            "category": "flag"
        },
        {
            "name": "flag-ly",
            "unicode": { "apple": "1F1F1-1F1FE", "google": "1F1F1-1F1FE", "twitter": "1F1F1-1F1FE" },
            "shortcode": "flag-ly",
            "category": "flag"
        },
        {
            "name": "flag-ma",
            "unicode": { "apple": "1F1F2-1F1E6", "google": "1F1F2-1F1E6", "twitter": "1F1F2-1F1E6" },
            "shortcode": "flag-ma",
            "category": "flag"
        },
        {
            "name": "flag-mc",
            "unicode": { "apple": "1F1F2-1F1E8", "google": "1F1F2-1F1E8", "twitter": "1F1F2-1F1E8" },
            "shortcode": "flag-mc",
            "category": "flag"
        },
        {
            "name": "flag-md",
            "unicode": { "apple": "1F1F2-1F1E9", "google": "1F1F2-1F1E9", "twitter": "1F1F2-1F1E9" },
            "shortcode": "flag-md",
            "category": "flag"
        },
        {
            "name": "flag-me",
            "unicode": { "apple": "1F1F2-1F1EA", "google": "1F1F2-1F1EA", "twitter": "1F1F2-1F1EA" },
            "shortcode": "flag-me",
            "category": "flag"
        },
        {
            "name": "flag-mg",
            "unicode": { "apple": "1F1F2-1F1EC", "google": "1F1F2-1F1EC", "twitter": "1F1F2-1F1EC" },
            "shortcode": "flag-mg",
            "category": "flag"
        },
        {
            "name": "flag-mh",
            "unicode": { "apple": "1F1F2-1F1ED", "google": "1F1F2-1F1ED", "twitter": "1F1F2-1F1ED" },
            "shortcode": "flag-mh",
            "category": "flag"
        },
        {
            "name": "flag-mk",
            "unicode": { "apple": "1F1F2-1F1F0", "google": "1F1F2-1F1F0", "twitter": "1F1F2-1F1F0" },
            "shortcode": "flag-mk",
            "category": "flag"
        },
        {
            "name": "flag-ml",
            "unicode": { "apple": "1F1F2-1F1F1", "google": "1F1F2-1F1F1", "twitter": "1F1F2-1F1F1" },
            "shortcode": "flag-ml",
            "category": "flag"
        },
        {
            "name": "flag-mm",
            "unicode": { "apple": "1F1F2-1F1F2", "google": "1F1F2-1F1F2", "twitter": "1F1F2-1F1F2" },
            "shortcode": "flag-mm",
            "category": "flag"
        },
        {
            "name": "flag-mn",
            "unicode": { "apple": "1F1F2-1F1F3", "google": "1F1F2-1F1F3", "twitter": "1F1F2-1F1F3" },
            "shortcode": "flag-mn",
            "category": "flag"
        },
        {
            "name": "flag-mo",
            "unicode": { "apple": "1F1F2-1F1F4", "google": "1F1F2-1F1F4", "twitter": "1F1F2-1F1F4" },
            "shortcode": "flag-mo",
            "category": "flag"
        },
        {
            "name": "flag-mp",
            "unicode": { "apple": "1F1F2-1F1F5", "google": "1F1F2-1F1F5", "twitter": "1F1F2-1F1F5" },
            "shortcode": "flag-mp",
            "category": "flag"
        },
        {
            "name": "flag-mr",
            "unicode": { "apple": "1F1F2-1F1F7", "google": "1F1F2-1F1F7", "twitter": "1F1F2-1F1F7" },
            "shortcode": "flag-mr",
            "category": "flag"
        },
        {
            "name": "flag-ms",
            "unicode": { "apple": "1F1F2-1F1F8", "google": "1F1F2-1F1F8", "twitter": "1F1F2-1F1F8" },
            "shortcode": "flag-ms",
            "category": "flag"
        },
        {
            "name": "flag-mt",
            "unicode": { "apple": "1F1F2-1F1F9", "google": "1F1F2-1F1F9", "twitter": "1F1F2-1F1F9" },
            "shortcode": "flag-mt",
            "category": "flag"
        },
        {
            "name": "flag-mu",
            "unicode": { "apple": "1F1F2-1F1FA", "google": "1F1F2-1F1FA", "twitter": "1F1F2-1F1FA" },
            "shortcode": "flag-mu",
            "category": "flag"
        },
        {
            "name": "flag-mv",
            "unicode": { "apple": "1F1F2-1F1FB", "google": "1F1F2-1F1FB", "twitter": "1F1F2-1F1FB" },
            "shortcode": "flag-mv",
            "category": "flag"
        },
        {
            "name": "flag-mw",
            "unicode": { "apple": "1F1F2-1F1FC", "google": "1F1F2-1F1FC", "twitter": "1F1F2-1F1FC" },
            "shortcode": "flag-mw",
            "category": "flag"
        },
        {
            "name": "flag-mx",
            "unicode": { "apple": "1F1F2-1F1FD", "google": "1F1F2-1F1FD", "twitter": "1F1F2-1F1FD" },
            "shortcode": "flag-mx",
            "category": "flag"
        },
        {
            "name": "flag-my",
            "unicode": { "apple": "1F1F2-1F1FE", "google": "1F1F2-1F1FE", "twitter": "1F1F2-1F1FE" },
            "shortcode": "flag-my",
            "category": "flag"
        },
        {
            "name": "flag-mz",
            "unicode": { "apple": "1F1F2-1F1FF", "google": "1F1F2-1F1FF", "twitter": "1F1F2-1F1FF" },
            "shortcode": "flag-mz",
            "category": "flag"
        },
        {
            "name": "flag-na",
            "unicode": { "apple": "1F1F3-1F1E6", "google": "1F1F3-1F1E6", "twitter": "1F1F3-1F1E6" },
            "shortcode": "flag-na",
            "category": "flag"
        },
        {
            "name": "flag-ne",
            "unicode": { "apple": "1F1F3-1F1EA", "google": "1F1F3-1F1EA", "twitter": "1F1F3-1F1EA" },
            "shortcode": "flag-ne",
            "category": "flag"
        },
        {
            "name": "flag-nf",
            "unicode": { "apple": "1F1F3-1F1EB", "google": "1F1F3-1F1EB", "twitter": "1F1F3-1F1EB" },
            "shortcode": "flag-nf",
            "category": "flag"
        },
        {
            "name": "flag-ng",
            "unicode": { "apple": "1F1F3-1F1EC", "google": "1F1F3-1F1EC", "twitter": "1F1F3-1F1EC" },
            "shortcode": "flag-ng",
            "category": "flag"
        },
        {
            "name": "flag-ni",
            "unicode": { "apple": "1F1F3-1F1EE", "google": "1F1F3-1F1EE", "twitter": "1F1F3-1F1EE" },
            "shortcode": "flag-ni",
            "category": "flag"
        },
        {
            "name": "flag-nl",
            "unicode": { "apple": "1F1F3-1F1F1", "google": "1F1F3-1F1F1", "twitter": "1F1F3-1F1F1" },
            "shortcode": "flag-nl",
            "category": "flag"
        },
        {
            "name": "flag-no",
            "unicode": { "apple": "1F1F3-1F1F4", "google": "1F1F3-1F1F4", "twitter": "1F1F3-1F1F4" },
            "shortcode": "flag-no",
            "category": "flag"
        },
        {
            "name": "flag-np",
            "unicode": { "apple": "1F1F3-1F1F5", "google": "1F1F3-1F1F5", "twitter": "1F1F3-1F1F5" },
            "shortcode": "flag-np",
            "category": "flag"
        },
        {
            "name": "flag-nr",
            "unicode": { "apple": "1F1F3-1F1F7", "google": "1F1F3-1F1F7", "twitter": "1F1F3-1F1F7" },
            "shortcode": "flag-nr",
            "category": "flag"
        },
        {
            "name": "flag-nu",
            "unicode": { "apple": "1F1F3-1F1FA", "google": "1F1F3-1F1FA", "twitter": "1F1F3-1F1FA" },
            "shortcode": "flag-nu",
            "category": "flag"
        },
        {
            "name": "flag-nz",
            "unicode": { "apple": "1F1F3-1F1FF", "google": "1F1F3-1F1FF", "twitter": "1F1F3-1F1FF" },
            "shortcode": "flag-nz",
            "category": "flag"
        },
        {
            "name": "flag-om",
            "unicode": { "apple": "1F1F4-1F1F2", "google": "1F1F4-1F1F2", "twitter": "1F1F4-1F1F2" },
            "shortcode": "flag-om",
            "category": "flag"
        },
        {
            "name": "flag-pa",
            "unicode": { "apple": "1F1F5-1F1E6", "google": "1F1F5-1F1E6", "twitter": "1F1F5-1F1E6" },
            "shortcode": "flag-pa",
            "category": "flag"
        },
        {
            "name": "flag-pe",
            "unicode": { "apple": "1F1F5-1F1EA", "google": "1F1F5-1F1EA", "twitter": "1F1F5-1F1EA" },
            "shortcode": "flag-pe",
            "category": "flag"
        },
        {
            "name": "flag-pf",
            "unicode": { "apple": "1F1F5-1F1EB", "google": "1F1F5-1F1EB", "twitter": "1F1F5-1F1EB" },
            "shortcode": "flag-pf",
            "category": "flag"
        },
        {
            "name": "flag-pg",
            "unicode": { "apple": "1F1F5-1F1EC", "google": "1F1F5-1F1EC", "twitter": "1F1F5-1F1EC" },
            "shortcode": "flag-pg",
            "category": "flag"
        },
        {
            "name": "flag-ph",
            "unicode": { "apple": "1F1F5-1F1ED", "google": "1F1F5-1F1ED", "twitter": "1F1F5-1F1ED" },
            "shortcode": "flag-ph",
            "category": "flag"
        },
        {
            "name": "flag-pk",
            "unicode": { "apple": "1F1F5-1F1F0", "google": "1F1F5-1F1F0", "twitter": "1F1F5-1F1F0" },
            "shortcode": "flag-pk",
            "category": "flag"
        },
        {
            "name": "flag-pl",
            "unicode": { "apple": "1F1F5-1F1F1", "google": "1F1F5-1F1F1", "twitter": "1F1F5-1F1F1" },
            "shortcode": "flag-pl",
            "category": "flag"
        },
        {
            "name": "flag-pn",
            "unicode": { "apple": "1F1F5-1F1F3", "google": "1F1F5-1F1F3", "twitter": "1F1F5-1F1F3" },
            "shortcode": "flag-pn",
            "category": "flag"
        },
        {
            "name": "flag-pr",
            "unicode": { "apple": "1F1F5-1F1F7", "google": "1F1F5-1F1F7", "twitter": "1F1F5-1F1F7" },
            "shortcode": "flag-pr",
            "category": "flag"
        },
        {
            "name": "flag-ps",
            "unicode": { "apple": "1F1F5-1F1F8", "google": "1F1F5-1F1F8", "twitter": "1F1F5-1F1F8" },
            "shortcode": "flag-ps",
            "category": "flag"
        },
        {
            "name": "flag-pt",
            "unicode": { "apple": "1F1F5-1F1F9", "google": "1F1F5-1F1F9", "twitter": "1F1F5-1F1F9" },
            "shortcode": "flag-pt",
            "category": "flag"
        },
        {
            "name": "flag-pw",
            "unicode": { "apple": "1F1F5-1F1FC", "google": "1F1F5-1F1FC", "twitter": "1F1F5-1F1FC" },
            "shortcode": "flag-pw",
            "category": "flag"
        },
        {
            "name": "flag-py",
            "unicode": { "apple": "1F1F5-1F1FE", "google": "1F1F5-1F1FE", "twitter": "1F1F5-1F1FE" },
            "shortcode": "flag-py",
            "category": "flag"
        },
        {
            "name": "flag-qa",
            "unicode": { "apple": "1F1F6-1F1E6", "google": "1F1F6-1F1E6", "twitter": "1F1F6-1F1E6" },
            "shortcode": "flag-qa",
            "category": "flag"
        },
        {
            "name": "flag-ro",
            "unicode": { "apple": "1F1F7-1F1F4", "google": "1F1F7-1F1F4", "twitter": "1F1F7-1F1F4" },
            "shortcode": "flag-ro",
            "category": "flag"
        },
        {
            "name": "flag-rs",
            "unicode": { "apple": "1F1F7-1F1F8", "google": "1F1F7-1F1F8", "twitter": "1F1F7-1F1F8" },
            "shortcode": "flag-rs",
            "category": "flag"
        },
        {
            "name": "ru",
            "unicode": { "apple": "1F1F7-1F1FA", "google": "1F1F7-1F1FA", "twitter": "1F1F7-1F1FA" },
            "shortcode": "ru",
            "category": "flag"
        },
        {
            "name": "flag-rw",
            "unicode": { "apple": "1F1F7-1F1FC", "google": "1F1F7-1F1FC", "twitter": "1F1F7-1F1FC" },
            "shortcode": "flag-rw",
            "category": "flag"
        },
        {
            "name": "flag-sa",
            "unicode": { "apple": "1F1F8-1F1E6", "google": "1F1F8-1F1E6", "twitter": "1F1F8-1F1E6" },
            "shortcode": "flag-sa",
            "category": "flag"
        },
        {
            "name": "flag-sb",
            "unicode": { "apple": "1F1F8-1F1E7", "google": "1F1F8-1F1E7", "twitter": "1F1F8-1F1E7" },
            "shortcode": "flag-sb",
            "category": "flag"
        },
        {
            "name": "flag-sc",
            "unicode": { "apple": "1F1F8-1F1E8", "google": "1F1F8-1F1E8", "twitter": "1F1F8-1F1E8" },
            "shortcode": "flag-sc",
            "category": "flag"
        },
        {
            "name": "flag-sd",
            "unicode": { "apple": "1F1F8-1F1E9", "google": "1F1F8-1F1E9", "twitter": "1F1F8-1F1E9" },
            "shortcode": "flag-sd",
            "category": "flag"
        },
        {
            "name": "flag-se",
            "unicode": { "apple": "1F1F8-1F1EA", "google": "1F1F8-1F1EA", "twitter": "1F1F8-1F1EA" },
            "shortcode": "flag-se",
            "category": "flag"
        },
        {
            "name": "flag-sg",
            "unicode": { "apple": "1F1F8-1F1EC", "google": "1F1F8-1F1EC", "twitter": "1F1F8-1F1EC" },
            "shortcode": "flag-sg",
            "category": "flag"
        },
        {
            "name": "flag-sh",
            "unicode": { "apple": "1F1F8-1F1ED", "google": "1F1F8-1F1ED", "twitter": "1F1F8-1F1ED" },
            "shortcode": "flag-sh",
            "category": "flag"
        },
        {
            "name": "flag-si",
            "unicode": { "apple": "1F1F8-1F1EE", "google": "1F1F8-1F1EE", "twitter": "1F1F8-1F1EE" },
            "shortcode": "flag-si",
            "category": "flag"
        },
        {
            "name": "flag-sj",
            "unicode": { "apple": "1F1F8-1F1EF", "google": "1F1F8-1F1EF", "twitter": "1F1F8-1F1EF" },
            "shortcode": "flag-sj",
            "category": "flag"
        },
        {
            "name": "flag-sk",
            "unicode": { "apple": "1F1F8-1F1F0", "google": "1F1F8-1F1F0", "twitter": "1F1F8-1F1F0" },
            "shortcode": "flag-sk",
            "category": "flag"
        },
        {
            "name": "flag-sl",
            "unicode": { "apple": "1F1F8-1F1F1", "google": "1F1F8-1F1F1", "twitter": "1F1F8-1F1F1" },
            "shortcode": "flag-sl",
            "category": "flag"
        },
        {
            "name": "flag-sm",
            "unicode": { "apple": "1F1F8-1F1F2", "google": "1F1F8-1F1F2", "twitter": "1F1F8-1F1F2" },
            "shortcode": "flag-sm",
            "category": "flag"
        },
        {
            "name": "flag-sn",
            "unicode": { "apple": "1F1F8-1F1F3", "google": "1F1F8-1F1F3", "twitter": "1F1F8-1F1F3" },
            "shortcode": "flag-sn",
            "category": "flag"
        },
        {
            "name": "flag-so",
            "unicode": { "apple": "1F1F8-1F1F4", "google": "1F1F8-1F1F4", "twitter": "1F1F8-1F1F4" },
            "shortcode": "flag-so",
            "category": "flag"
        },
        {
            "name": "flag-sr",
            "unicode": { "apple": "1F1F8-1F1F7", "google": "1F1F8-1F1F7", "twitter": "1F1F8-1F1F7" },
            "shortcode": "flag-sr",
            "category": "flag"
        },
        {
            "name": "flag-ss",
            "unicode": { "apple": "1F1F8-1F1F8", "google": "1F1F8-1F1F8", "twitter": "1F1F8-1F1F8" },
            "shortcode": "flag-ss",
            "category": "flag"
        },
        {
            "name": "flag-st",
            "unicode": { "apple": "1F1F8-1F1F9", "google": "1F1F8-1F1F9", "twitter": "1F1F8-1F1F9" },
            "shortcode": "flag-st",
            "category": "flag"
        },
        {
            "name": "flag-sv",
            "unicode": { "apple": "1F1F8-1F1FB", "google": "1F1F8-1F1FB", "twitter": "1F1F8-1F1FB" },
            "shortcode": "flag-sv",
            "category": "flag"
        },
        {
            "name": "flag-sx",
            "unicode": { "apple": "1F1F8-1F1FD", "google": "1F1F8-1F1FD", "twitter": "1F1F8-1F1FD" },
            "shortcode": "flag-sx",
            "category": "flag"
        },
        {
            "name": "flag-sy",
            "unicode": { "apple": "1F1F8-1F1FE", "google": "1F1F8-1F1FE", "twitter": "1F1F8-1F1FE" },
            "shortcode": "flag-sy",
            "category": "flag"
        },
        {
            "name": "flag-sz",
            "unicode": { "apple": "1F1F8-1F1FF", "google": "1F1F8-1F1FF", "twitter": "1F1F8-1F1FF" },
            "shortcode": "flag-sz",
            "category": "flag"
        },
        {
            "name": "flag-ta",
            "unicode": { "apple": "1F1F9-1F1E6", "google": "1F1F9-1F1E6", "twitter": "1F1F9-1F1E6" },
            "shortcode": "flag-ta",
            "category": "flag"
        },
        {
            "name": "flag-tc",
            "unicode": { "apple": "1F1F9-1F1E8", "google": "1F1F9-1F1E8", "twitter": "1F1F9-1F1E8" },
            "shortcode": "flag-tc",
            "category": "flag"
        },
        {
            "name": "flag-td",
            "unicode": { "apple": "1F1F9-1F1E9", "google": "1F1F9-1F1E9", "twitter": "1F1F9-1F1E9" },
            "shortcode": "flag-td",
            "category": "flag"
        },
        {
            "name": "flag-tg",
            "unicode": { "apple": "1F1F9-1F1EC", "google": "1F1F9-1F1EC", "twitter": "1F1F9-1F1EC" },
            "shortcode": "flag-tg",
            "category": "flag"
        },
        {
            "name": "flag-th",
            "unicode": { "apple": "1F1F9-1F1ED", "google": "1F1F9-1F1ED", "twitter": "1F1F9-1F1ED" },
            "shortcode": "flag-th",
            "category": "flag"
        },
        {
            "name": "flag-tj",
            "unicode": { "apple": "1F1F9-1F1EF", "google": "1F1F9-1F1EF", "twitter": "1F1F9-1F1EF" },
            "shortcode": "flag-tj",
            "category": "flag"
        },
        {
            "name": "flag-tk",
            "unicode": { "apple": "1F1F9-1F1F0", "google": "1F1F9-1F1F0", "twitter": "1F1F9-1F1F0" },
            "shortcode": "flag-tk",
            "category": "flag"
        },
        {
            "name": "flag-tl",
            "unicode": { "apple": "1F1F9-1F1F1", "google": "1F1F9-1F1F1", "twitter": "1F1F9-1F1F1" },
            "shortcode": "flag-tl",
            "category": "flag"
        },
        {
            "name": "flag-tm",
            "unicode": { "apple": "1F1F9-1F1F2", "google": "1F1F9-1F1F2", "twitter": "1F1F9-1F1F2" },
            "shortcode": "flag-tm",
            "category": "flag"
        },
        {
            "name": "flag-tn",
            "unicode": { "apple": "1F1F9-1F1F3", "google": "1F1F9-1F1F3", "twitter": "1F1F9-1F1F3" },
            "shortcode": "flag-tn",
            "category": "flag"
        },
        {
            "name": "flag-to",
            "unicode": { "apple": "1F1F9-1F1F4", "google": "1F1F9-1F1F4", "twitter": "1F1F9-1F1F4" },
            "shortcode": "flag-to",
            "category": "flag"
        },
        {
            "name": "flag-tr",
            "unicode": { "apple": "1F1F9-1F1F7", "google": "1F1F9-1F1F7", "twitter": "1F1F9-1F1F7" },
            "shortcode": "flag-tr",
            "category": "flag"
        },
        {
            "name": "flag-tt",
            "unicode": { "apple": "1F1F9-1F1F9", "google": "1F1F9-1F1F9", "twitter": "1F1F9-1F1F9" },
            "shortcode": "flag-tt",
            "category": "flag"
        },
        {
            "name": "flag-tv",
            "unicode": { "apple": "1F1F9-1F1FB", "google": "1F1F9-1F1FB", "twitter": "1F1F9-1F1FB" },
            "shortcode": "flag-tv",
            "category": "flag"
        },
        {
            "name": "flag-tw",
            "unicode": { "apple": "1F1F9-1F1FC", "google": "1F1F9-1F1FC", "twitter": "1F1F9-1F1FC" },
            "shortcode": "flag-tw",
            "category": "flag"
        },
        {
            "name": "flag-tz",
            "unicode": { "apple": "1F1F9-1F1FF", "google": "1F1F9-1F1FF", "twitter": "1F1F9-1F1FF" },
            "shortcode": "flag-tz",
            "category": "flag"
        },
        {
            "name": "flag-ua",
            "unicode": { "apple": "1F1FA-1F1E6", "google": "1F1FA-1F1E6", "twitter": "1F1FA-1F1E6" },
            "shortcode": "flag-ua",
            "category": "flag"
        },
        {
            "name": "flag-ug",
            "unicode": { "apple": "1F1FA-1F1EC", "google": "1F1FA-1F1EC", "twitter": "1F1FA-1F1EC" },
            "shortcode": "flag-ug",
            "category": "flag"
        },
        {
            "name": "flag-um",
            "unicode": { "apple": "1F1FA-1F1F2", "google": "1F1FA-1F1F2", "twitter": "1F1FA-1F1F2" },
            "shortcode": "flag-um",
            "category": "flag"
        },
        {
            "name": "us",
            "unicode": { "apple": "1F1FA-1F1F8", "google": "1F1FA-1F1F8", "twitter": "1F1FA-1F1F8" },
            "shortcode": "us",
            "category": "flag"
        },
        {
            "name": "flag-uy",
            "unicode": { "apple": "1F1FA-1F1FE", "google": "1F1FA-1F1FE", "twitter": "1F1FA-1F1FE" },
            "shortcode": "flag-uy",
            "category": "flag"
        },
        {
            "name": "flag-uz",
            "unicode": { "apple": "1F1FA-1F1FF", "google": "1F1FA-1F1FF", "twitter": "1F1FA-1F1FF" },
            "shortcode": "flag-uz",
            "category": "flag"
        },
        {
            "name": "flag-va",
            "unicode": { "apple": "1F1FB-1F1E6", "google": "1F1FB-1F1E6", "twitter": "1F1FB-1F1E6" },
            "shortcode": "flag-va",
            "category": "flag"
        },
        {
            "name": "flag-vc",
            "unicode": { "apple": "1F1FB-1F1E8", "google": "1F1FB-1F1E8", "twitter": "1F1FB-1F1E8" },
            "shortcode": "flag-vc",
            "category": "flag"
        },
        {
            "name": "flag-ve",
            "unicode": { "apple": "1F1FB-1F1EA", "google": "1F1FB-1F1EA", "twitter": "1F1FB-1F1EA" },
            "shortcode": "flag-ve",
            "category": "flag"
        },
        {
            "name": "flag-vg",
            "unicode": { "apple": "1F1FB-1F1EC", "google": "1F1FB-1F1EC", "twitter": "1F1FB-1F1EC" },
            "shortcode": "flag-vg",
            "category": "flag"
        },
        {
            "name": "flag-vi",
            "unicode": { "apple": "1F1FB-1F1EE", "google": "1F1FB-1F1EE", "twitter": "1F1FB-1F1EE" },
            "shortcode": "flag-vi",
            "category": "flag"
        },
        {
            "name": "flag-vn",
            "unicode": { "apple": "1F1FB-1F1F3", "google": "1F1FB-1F1F3", "twitter": "1F1FB-1F1F3" },
            "shortcode": "flag-vn",
            "category": "flag"
        },
        {
            "name": "flag-vu",
            "unicode": { "apple": "1F1FB-1F1FA", "google": "1F1FB-1F1FA", "twitter": "1F1FB-1F1FA" },
            "shortcode": "flag-vu",
            "category": "flag"
        },
        {
            "name": "flag-ws",
            "unicode": { "apple": "1F1FC-1F1F8", "google": "1F1FC-1F1F8", "twitter": "1F1FC-1F1F8" },
            "shortcode": "flag-ws",
            "category": "flag"
        },
        {
            "name": "flag-ye",
            "unicode": { "apple": "1F1FE-1F1EA", "google": "1F1FE-1F1EA", "twitter": "1F1FE-1F1EA" },
            "shortcode": "flag-ye",
            "category": "flag"
        },
        {
            "name": "flag-za",
            "unicode": { "apple": "1F1FF-1F1E6", "google": "1F1FF-1F1E6", "twitter": "1F1FF-1F1E6" },
            "shortcode": "flag-za",
            "category": "flag"
        },
        {
            "name": "flag-zm",
            "unicode": { "apple": "1F1FF-1F1F2", "google": "1F1FF-1F1F2", "twitter": "1F1FF-1F1F2" },
            "shortcode": "flag-zm",
            "category": "flag"
        },
        {
            "name": "flag-zw",
            "unicode": { "apple": "1F1FF-1F1FC", "google": "1F1FF-1F1FC", "twitter": "1F1FF-1F1FC" },
            "shortcode": "flag-zw",
            "category": "flag"
        },
        {
            "name": "flag-england",
            "unicode": {
                "apple": "1F3F4-E0067-E0062-E0065-E006E-E0067-E007F",
                "google": "1F3F4-E0067-E0062-E0065-E006E-E0067-E007F",
                "twitter": "1F3F4-E0067-E0062-E0065-E006E-E0067-E007F"
            },
            "shortcode": "flag-england",
            "category": "flag"
        },
        {
            "name": "flag-scotland",
            "unicode": {
                "apple": "1F3F4-E0067-E0062-E0073-E0063-E0074-E007F",
                "google": "1F3F4-E0067-E0062-E0073-E0063-E0074-E007F",
                "twitter": "1F3F4-E0067-E0062-E0073-E0063-E0074-E007F"
            },
            "shortcode": "flag-scotland",
            "category": "flag"
        },
        {
            "name": "flag-wales",
            "unicode": {
                "apple": "1F3F4-E0067-E0062-E0077-E006C-E0073-E007F",
                "google": "1F3F4-E0067-E0062-E0077-E006C-E0073-E007F",
                "twitter": "1F3F4-E0067-E0062-E0077-E006C-E0073-E007F"
            },
            "shortcode": "flag-wales",
            "category": "flag"
        }
    ];
});
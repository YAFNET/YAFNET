/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
 * https://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Tests.UITests.Mockup;

public record MockupForum(string Name, string Description);

public record MockupCategory(string Name, MockupForum[] Forums);

public record MockupTopic(
    string Forum,
    string Author,
    string Subject,
    string Message,
    (string Author, string Message)[] Replies,
    bool Showcase = false);

/// <summary>
/// The demo community of the website mockup image (a photography forum).
/// The topics are created in this order, so the last ones are the latest posts.
/// </summary>
public static class MockupContent
{
    public const string BoardName = "Focus Photography Community";

    public const string Admin = "Alex";

    /// <summary>
    /// The members, the first two are registered by the fixture after the install.
    /// </summary>
    public readonly static string[] Members = ["MariaK", "JonasB", "LenaPhotos", "TomWalker", "PriyaS"];

    public readonly static MockupCategory[] Categories =
    [
        new("Community",
        [
            new("Announcements", "News and updates from the team"),
            new("Introductions", "New here? Say hello and tell us what you like to shoot"),
            new("Off-Topic Lounge", "Everything that is not about photography")
        ]),
        new("Photography",
        [
            new("Gear Talk", "Cameras, lenses, bags and everything in between"),
            new("Techniques and Tips", "Composition, light and shooting techniques"),
            new("Photo Critique", "Share your shots and get honest, friendly feedback"),
            new("Post-Processing", "Lightroom, Capture One, darktable and friends")
        ]),
        new("Marketplace",
        [
            new("Buy and Sell", "Used gear from members for members"),
            new("Workshops and Meetups", "Photo walks, workshops and local meetups")
        ])
    ];

    /// <summary>
    /// The topic that is written on the tablet (not posted).
    /// </summary>
    public readonly static MockupTopic Draft = new(
        "Techniques and Tips",
        Admin,
        "How do you keep horizons straight without a tripod?",
        "I shoot a lot of seascapes handheld and half of my horizons end up slightly tilted.\n\nDo you use the level in the viewfinder, a grid overlay, or do you just fix it later in post?",
        []);

    public readonly static MockupTopic[] Topics =
    [
        new("Announcements", Admin, "Welcome to the Focus Photography Community!",
            "Hi everyone and welcome!\n\nThis is a place to share your work, ask questions and learn from each other, no matter if you shoot with a phone or a medium format camera.\n\nPlease take a minute to introduce yourself in the Introductions forum. Happy shooting!",
            [
                ("MariaK", "Thanks for setting this up, Alex! Looking forward to the photo walks."),
                ("TomWalker", "Great to have a place without the noise of the big social networks.")
            ]),
        new("Introductions", "PriyaS", "Hi from Lisbon, street and travel photographer",
            "Hello all! I'm Priya, I live in Lisbon and mostly shoot street and travel photography with a small rangefinder style camera.\n\nHappy to be here and to learn more about editing.",
            [
                ("LenaPhotos", "Welcome Priya! Lisbon must be a dream for street photography, all that light."),
                ("JonasB", "Welcome! Please share some of your street shots in the critique forum.")
            ]),
        new("Techniques and Tips", "LenaPhotos", "Golden hour vs. blue hour for cityscapes",
            "I always end up shooting cityscapes at golden hour, but I keep seeing amazing blue hour shots.\n\nWhat do you prefer, and how do you handle the mixed light of street lamps and the sky?",
            [
                ("TomWalker", "Blue hour for me. The sky and the city lights are balanced for maybe 20 minutes, so be there early and set up before."),
                ("JonasB", "Bracket your exposures and blend them later, that saves a lot of shots with burnt out street lamps.")
            ]),
        new("Photo Critique", "TomWalker", "First attempt at long exposure, feedback welcome",
            "Took my new ND filter to the coast last weekend. 30 seconds at f/11, ISO 100.\n\nThe water came out nice and smooth, but the rocks in the foreground look a bit soft. Any ideas what went wrong?",
            [
                ("LenaPhotos", "Nice mood! Soft rocks are often wind shaking the tripod, try hanging your bag on the center column."),
                ("MariaK", "Also check if image stabilization was on, it should be off on a tripod.")
            ]),
        new("Post-Processing", "MariaK", "Lightroom presets: worth it or a crutch?",
            "I bought a preset pack last year and use it on almost every photo now. Lately I feel all my pictures look the same.\n\nDo you use presets, or do you edit every photo from scratch?",
            [
                ("PriyaS", "I made my own presets as a starting point, then adjust white balance and exposure per photo."),
                ("JonasB", "Presets are fine, but learn what every slider does first. Then you know when to break the look."),
                (Admin, "Good discussion! Maybe we start a monthly editing challenge with the same raw file for everyone?")
            ]),
        new("Buy and Sell", "TomWalker", "Selling: 50mm f/1.8 prime, like new",
            "Selling my 50mm f/1.8 because I upgraded to the f/1.4. Barely used, with both caps and the original box.\n\nPickup in Hamburg or shipping within the EU.",
            [
                ("JonasB", "Sent you a private message!")
            ]),
        new("Workshops and Meetups", Admin, "Photo walk in Hamburg on Saturday",
            "Let's meet at the Landungsbrücken at 9:00 and walk through the harbour and the Speicherstadt.\n\nAll levels and all cameras are welcome. Reply here if you want to join!",
            [
                ("MariaK", "Count me in!"),
                ("PriyaS", "I'll be in Hamburg that weekend, so I'm in."),
                ("LenaPhotos", "Joining as well, I'll bring my old film camera.")
            ]),
        new("Off-Topic Lounge", "PriyaS", "What are you listening to while editing?",
            "I need some new music for long editing sessions. Currently it's mostly lo-fi and old jazz records.\n\nWhat's on your playlist?",
            [
                ("TomWalker", "Podcasts about photography, then I feel productive twice.")
            ]),
        new("Gear Talk", "MariaK", "Mirrorless vs. DSLR in 2026, still a debate?",
            "My old DSLR is slowly giving up and I wonder if there is any reason left to buy a new one.\n\nAnyone still shooting DSLR by choice?",
            [
                ("JonasB", "The optical viewfinder is the only thing I miss. The eye autofocus of the mirrorless cameras is just too good."),
                (Admin, "Battery life used to be the big argument, but that is mostly solved now.")
            ]),
        new("Gear Talk", "JonasB", "What's in your camera bag this year?",
            "Let's see your kit! Mine for this year:\n\n- Mirrorless body with a 24-70mm f/4\n- 35mm f/1.8 for street and low light\n- Small travel tripod and a 6 stop ND filter\n\nWhat do you carry?",
            [
                ("MariaK", "One body, one lens: 35mm. It forces me to move and think about the frame."),
                ("LenaPhotos", "A 70-200mm for landscapes, compressing layers of hills is my favourite thing."),
                ("TomWalker", "Too much, as always. I need to learn from Maria."),
                ("PriyaS", "A compact camera and a spare battery. Light bag, happy back!")
            ],
            Showcase: true)
    ];
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dontDestroyScript : MonoBehaviour {

    public bool isStandard;
    public int deckIDNumber;

    public List<string> deckTitles = new List<string>();
    public List<string> deckContents = new List<string>();

    public List<string> correctWords = new List<string>();
    public List<string> skipWords = new List<string>();

    public int correctCount = 0;
    public int skipCount = 0;
        
    void Awake() {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start() {
        deckTitles.Add("Food"); deckContents.Add("carrots[word]salad[word]fish[word]meat[word]apples[word]bananas[word]oranges[word]pears[word]apricots[word]french fries[word]pasta[word]chocolate[word]cookies[word]cake[word]milk[word]cheese[word]tea[word]coffee[word]green beans[word]peas[word]donut[word]broccoli[word]pizza[word]pop corn[word]soup[word]ice cream[word]juice");
        deckTitles.Add("Charades"); deckContents.Add("Ketchup[word]Sitting in a chair[word]Knocking on a door[word]Flying a Kite[word]Eating a burger[word]Painting[word]Fishing[word]Sawing a log[word]Snoring[word]Braiding hair[word]Mowing the lawn[word]Washing the floors[word]Cutting food with a knife[word]Refridgerator[word]Putting on a coat[word]Shoveling snow[word]Ringing a doorbell[word]Steering wheel[word]Listening to the radio[word]Playing in a football game[word]Golfing[word]Glasses[word]Running[word]Skipping[word]Jumping[word]Cleaning out ears[word]Belt[word]Couch[word]White[word]Purple[word]Clouds[word]Raining[word]Tornado[word]Lightning[word]Taking a picture[word]Printing from a computer[word]Turning on a light[word]Pillow[word]Throwing a frisbee[word]Brushing teeth[word]Playing Battlefield 3[word]Floss[word]Taking a bath[word]Rubbing dad's feet[word]Push-up[word]Jumping jacks[word]Pouring milk[word]Blowing your nose[word]Changing a light bulb[word]Cooking[word]Grilling[word]Hanging a picture[word]Broom[word]Grass[word]Plane[word]Rock[word]Leaf[word]Wood[word]Wall[word]Dirt[word]Snowball[word]Killing a bug[word]Bird[word]Rocket[word]Steps[word]Carpet[word]Puzzle[word]Sign Language[word]Opening up the garage door[word]Putting a worm on a hook[word]Wink[word]Nail Polish[word]Shampoo[word]Lipstick[word]Chin[word]Cleaning[word]Drums[word]Cartwheel[word]Summer[word]Head stand[word]Talking on the phone[word]Buying something[word]Salt[word]Pepper[word]Socks[word]Underwear[word]Shorts[word]Dress[word]Woman[word]Man[word]Bald[word]Long hair[word]Eye lashes[word]Eye brow[word]Table leg[word]Seat[word]Back[word]Stick[word]Marshmellow[word]Soup[word]Tomato[word]Haircut[word]Toe nail[word]Cutting Fingernails[word]Washing the dishes[word]Plate[word]Hopscotch[word]Basketball[word]Tennis[word]Sky[word]Moon[word]Sun[word]Bowl[word]Book[word]Reading[word]Turning up the volumn[word]Water[word]Sink[word]Shower[word]Vacumming[word]Cuddle[word]Kiss[word]Hug[word]High 3[word]Balloon[word]Pillow case[word]Nostril[word]Blood[word]Snot[word]Ear wax[word]Spit[word]Chewing gum[word]Typing[word]Thinking[word]Blink[word]Bottom of your foot[word]Popcorn[word]Butter[word]Praying[word]Toss[word]Folding Laundry[word]Starting up Netflix[word]Stop drop and roll[word]Dancing[word]Candles[word]Christmas Tree[word]Present[word]Lock[word]Window[word]Shop[word]Smile[word]Sad[word]Laugh[word]Frog[word]Lion[word]Girraff[word]Dinosaur[word]Snake[word]Fish[word]Turtle[word]Waking up[word]Race[word]Spray bottle[word]Tie");

        // **** standard ****//  
        deckTitles.Add("Chapter 1"); deckContents.Add("avid[word]brusque[word]concise[word]demean[word]despicable[word]emulate[word]evoke[word]excruciating[word]inaugurate[word]pervade[word]proprietor[word]pseudonym[word]rebuff[word]resilient[word]turbulent");
        deckTitles.Add("Chapter 2"); deckContents.Add("abrasion[word]clad[word]corroborate[word]cursory[word]dehydrate[word]derive[word]electrify[word]endeavor[word]gingerly[word]grimace[word]gruesome[word]inventory[word]simulate[word]succumb[word]surmise");
        deckTitles.Add("Chapter 3"); deckContents.Add("anonymous[word]anthology[word]conjecture[word]disposition[word]encompass[word]extricate[word]generation[word]guile[word]imperative[word]instill[word]modify[word]pivot[word]prevalent[word]recur[word]spontaneous");
        deckTitles.Add("Chapter 4"); deckContents.Add("abhor[word]affable[word]amiss[word]despondent[word]entreat[word]haunt[word]impel[word]interminable[word]irascible[word]profound[word]recluse[word]reverberate[word]sage[word]tirade[word]tremulous");
        deckTitles.Add("Chapter 5"); deckContents.Add("audacious[word]confiscate[word]conscientious[word]depict[word]embark[word]inkling[word]lackadaisical[word]mutiny[word]pilfer[word]profusion[word]prudent[word]rankle[word]rebuke[word]serene[word]slovenly");
        deckTitles.Add("Chapter 6"); deckContents.Add("anarchy[word]apprehend[word]arraign[word]assimilate[word]bizarre[word]calamity[word]conspire[word]dissension[word]elapse[word]imminent[word]interrogate[word]lionize[word]meticulous[word]shackle[word]swelter");
        deckTitles.Add("Chapter 7"); deckContents.Add("claustrophobia[word]colleague[word]condescend[word]contingent[word]daunt[word]deluge[word]dispel[word]dub[word]fanfare[word]fledgling[word]inane[word]mettle[word]negligible[word]protract[word]replica");
        deckTitles.Add("Chapter 8"); deckContents.Add("adept[word]audible[word]azure[word]banter[word]capacious[word]copious[word]crucial[word]decelerate[word]deploy[word]facilitate[word]fastidious[word]fitful[word]grapple[word]pang[word]precede");
        deckTitles.Add("Chapter 9"); deckContents.Add("abet[word]agile[word]allot[word]balmy[word]congregate[word]divert[word]humdrum[word]influx[word]intricate[word]memento[word]query[word]sporadic[word]staple[word]tumult[word]unseemly");
        deckTitles.Add("Chapter 10"); deckContents.Add("abject[word]advocate[word]atrocity[word]commemorate[word]dialect[word]dire[word]elite[word]enhance[word]flagrant[word]languish[word]mute[word]raze[word]reprisal[word]turmoil[word]wreak");
        deckTitles.Add("Chapter 11"); deckContents.Add("augment[word]benign[word]connoisseur[word]discern[word]embellish[word]execute[word]exemplify[word]grotesque[word]hallowed[word]impersonate[word]malevolent[word]ornate[word]pastoral[word]precarious[word]renown");
        deckTitles.Add("Chapter 12"); deckContents.Add("accede[word]affluent[word]arbitrary[word]artisan[word]dismantle[word]immerse[word]irksome[word]legacy[word]ostentatious[word]panorama[word]philanthropy[word]prestige[word]prolific[word]reticent[word]tycoon");
        deckTitles.Add("Chapter 13"); deckContents.Add("ardent[word]assail[word]asset[word]barter[word]bonanza[word]contagious[word]contemplate[word]deter[word]flair[word]forfeit[word]innovation[word]mania[word]stymie[word]synonymous[word]wrangle");
        deckTitles.Add("Chapter 14"); deckContents.Add("congenial[word]decipher[word]dissect[word]enigma[word]ineffectual[word]infallible[word]irrepressible[word]luminous[word]millennium[word]mire[word]pestilence[word]stagnate[word]sublime[word]vie[word]voluminous");
        deckTitles.Add("Chapter 15"); deckContents.Add("ascertain[word]chastise[word]cull[word]defer[word]desist[word]discredit[word]encroach[word]foreboding[word]humane[word]irrational[word]lurid[word]perpetuate[word]restive[word]stamina[word]surveillance");
        deckTitles.Add("Chapter 16"); deckContents.Add("alleviate[word]antidote[word]bedlam[word]cajole[word]glib[word]haggard[word]immaculate[word]incessant[word]indulgent[word]loll[word]pittance[word]pungent[word]rue[word]strident[word]vehement");
        deckTitles.Add("Chapter 17"); deckContents.Add("accord[word]affirm[word]bequeath[word]citadel[word]confer[word]coup[word]dignitary[word]embroil[word]epoch[word]impeccable[word]institute[word]patriarch[word]rapport[word]renounce[word]rhetoric");
        deckTitles.Add("Chapter 18"); deckContents.Add("aperture[word]cache[word]combustible[word]delegate[word]inclement[word]indelible[word]malady[word]memoir[word]paramount[word]rectify[word]requisite[word]squeamish[word]tract[word]tribulation[word]vignette");
        deckTitles.Add("Chapter 19"); deckContents.Add("bulwark[word]culminate[word]engulf[word]feasible[word]glut[word]havoc[word]impregnable[word]indefatigable[word]onslaught[word]phenomenon[word]picturesque[word]simultaneous[word]stipulate[word]susceptible[word]wrest");
        deckTitles.Add("Chapter 20"); deckContents.Add("alienate[word]fervent[word]forbearance[word]gullible[word]hindrance[word]inflammatory[word]ordain[word]ovation[word]overt[word]recant[word]rejoinder[word]reproach[word]servile[word]surpass[word]vilify");
    }

}

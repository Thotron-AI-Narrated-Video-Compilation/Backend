import sys
import json     
from youtube_transcript_api import YouTubeTranscriptApi

#from gensim.models import Word2Vec
#from nltk.tokenize import word_tokenize
#import nltk
import numpy as np
#from sklearn.metrics.pairwise import cosine_similarity




def fetch_transcript(video_id):
    try:
        transcript = YouTubeTranscriptApi.get_transcript(video_id,languages=['en','ar','de'])
        for element in transcript:
            element.update({"end" : element["start"]+element["duration"]})
        return transcript
    
    except Exception as e:
        return {"error":str(e)}






if __name__ == "__main__":
    video_id = sys.argv[1]
    transcript = fetch_transcript(video_id)

    #get transcript only
    if(len(sys.argv) == 2):
        print(transcript)

    #find best match promot
    else:
        prompt = sys.argv[2]
        caption = best_match(video_id,prompt)
        print(json.dumps(caption))

            

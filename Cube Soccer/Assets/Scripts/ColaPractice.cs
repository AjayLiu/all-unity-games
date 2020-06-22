using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ColaPractice : MonoBehaviour {

	public int result;

	void Start(){
		result = CalculateColasDrinkable(8);
	}	

	int totalColas;
	int redeemedColas;
    int emptyColas;

	bool startCalculation;

    int CalculateColasDrinkable(int colas) {
       
        if(colas > 2 && colas <= 100) {
        	totalColas = colas;
            startCalculation = true;
        } else { // no need to calculate if colas is 2 or less
            startCalculation = false;
            return colas;
        }

        while (totalColas >= 3 && startCalculation == true) {
			emptyColas = totalColas % 3;
            redeemedColas = (totalColas + emptyColas) / 3;
        }

		return colas + redeemedColas;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBXSwingReward : MonoBehaviour
{
    public float Power_N2_Reward(float distance)
    {
    	if(distance < 0.1)
    		return 1f;
    	else if(distance < 2)
    	{
    		return (1 / (distance * distance * 100));
    	}
    	else
    		return 0f;
    }

    public float Power_N3_Reward(float distance)
    {
        float distance_max = 2.8f;
        if(distance < 0.1)
            return 1f;
        else
        {
            return (1 / (distance * distance * distance * 1000));
        }
    }

    public float Power_P04_Reward(float distance)
    {
    	float distance_max = 3f;
    	float reward = 1 - Mathf.Pow(distance / distance_max, 0.4f);
    	return reward;
    }

    public float Power_N3_Penalty(float distance)
    {
        float distance_max = 3f;
        return -Mathf.Pow(distance/distance_max, 3f);
    }

    public float Power_Hybrid(float distance)
    {
        float distance_max = 2.8f;
        if(distance < 0.1)
            return 1f;
        else if(distance < 0.5)
            return (1 / (distance * distance * 100));
        else
            return -Mathf.Pow(distance/distance_max, 3f);
    }

    public float Power_Hybrid_NP2(float distance)
    {
        float distance_max = 2.8f;
        if(distance < 0.1)
            return 1f;
        else if(distance < 0.5)
            return (1 / (distance * distance * 100));
        else
            return -Mathf.Pow(distance/distance_max, 2f);
    }

    public float Power_Hybrid_PP3_NP2(float distance)
    {
        float distance_max = 2.8f;
        if(distance < 0.1)
            return 1f;
        else if(distance < 0.5)
            return (1 / (distance * distance * distance * 1000));
        else
            return -Mathf.Pow(distance/distance_max, 2f);
    }

    public float Power_Hybrid_PP4_NP2(float distance)
    {
        float distance_max = 2.8f;
        if(distance < 0.1)
            return 1f;
        else if(distance < 0.5)
            return (1 / (distance * distance * distance * distance * 10000));
        else
            return -Mathf.Pow(distance/distance_max, 2f);
    }

    public float Power_Hybrid_PP4_NP3(float distance)
    {
        float distance_max = 2.8f;
        if(distance < 0.1)
            return 1f;
        else if(distance < 0.5)
            return (1 / (distance * distance * distance * distance * 10000));
        else
            return -Mathf.Pow(distance/distance_max, 3f);
    }

    public float Power_Hybrid_PP3_NP3(float distance)
    {
        float distance_max = 2.8f;
        if(distance < 0.1)
            return 1f;
        else if(distance < 0.5)
            return (1 / (distance * distance * distance * 1000));
        else
            return -Mathf.Pow(distance/distance_max, 3f);
    }
}

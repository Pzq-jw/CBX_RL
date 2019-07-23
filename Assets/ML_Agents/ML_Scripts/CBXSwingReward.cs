using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBXSwingReward : MonoBehaviour
{

    public float distance_max = 2.8f;

    public float Default_Reward(float distance)
    {
        return -3.14f;
    }

    public float Test_Reward(float distance)
    {
        return -1.201f;
    }
    
    public float New_Hybrid(float distance)
    {
        if(distance < 0.1f)
            return 1f;
        else if(distance <= 0.5f)
        {
            return (1 / (distance * distance * 100));
        }
        else
        {
            return -0.1f;
        }
    }

    public float Power_N2_Reward(float distance)
    {
    	if(distance < 0.1)
    		return 1f;
    	else if(distance <= 0.5)
    	{
    		return (1 / (distance * distance * 100));
    	}
    	else
    		return 0f;
    }

    public float Power_N3_Reward(float distance)
    {
        if(distance < 0.1)
            return 1f;
        else
        {
            return (1 / (distance * distance * distance * 1000));
        }
    }

    public float Power_P04_Reward(float distance)
    {
    	float r = 1 - Mathf.Pow(distance / distance_max, 0.4f);
    	return r;
    }

    public float Power_N3_Penalty(float distance)
    {
        return -Mathf.Pow(distance/distance_max, 3f);
    }

    public float Power_Hybrid(float distance)
    {
        if(distance < 0.1)
            return 1f;
        else if(distance < 0.5)
            return (1 / (distance * distance * 100));
        else
            return -Mathf.Pow((distance - 0.5f)/(distance_max - 0.5f), 2f);
    }

    public float Power_Hybrid_NP2(float distance)
    {
        if(distance < 0.1)
            return 1f;
        else if(distance < 0.5)
            return (1 / (distance * distance * 100));
        else
            return -Mathf.Pow(distance/distance_max, 2f);
    }

    public float Power_Hybrid_PP3_NP2(float distance)
    {
        if(distance < 0.1)
            return 1f;
        else if(distance < 0.5)
            return (1 / (distance * distance * distance * 1000));
        else
            return -Mathf.Pow(distance/distance_max, 2f);
    }

    public float Power_Hybrid_PP4_NP2(float distance)
    {
        if(distance < 0.1)
            return 1f;
        else if(distance < 0.5)
            return (1 / (distance * distance * distance * distance * 10000));
        else
            return -Mathf.Pow(distance/distance_max, 2f);
    }

    public float Power_Hybrid_PP4_NP3(float distance)
    {
        if(distance < 0.1)
            return 1f;
        else if(distance < 0.5)
            return (1 / (distance * distance * distance * distance * 10000));
        else
            return -Mathf.Pow(distance/distance_max, 3f);
    }

    public float Power_Hybrid_PP3_NP3(float distance)
    {
        if(distance < 0.1)
            return 1f;
        else if(distance < 0.5)
            return (1 / (distance * distance * distance * 1000));
        else
            return -Mathf.Pow(distance/distance_max, 3f);
    }

    public float Dead_Hybrid(float distance)
    {
        if(distance <= 0.1)
            return 1f;
        else
            return -Mathf.Pow(distance/distance_max, 3f);
    }































}

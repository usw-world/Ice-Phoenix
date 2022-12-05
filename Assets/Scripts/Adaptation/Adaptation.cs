 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adaptation : MonoBehaviour {
    public int remainingPoint { get; private set; }
    public enum Type {
        /// <summary>플레이어 기본 공격, 점프 공격 관련</summary>
        Power=0,
        /// <summary>플레이어 이동 속도, 회피 속도 관련</summary>
        Movement=1,
        /// <summary>플레이어 공격 속도 관련</summary>
        Fast=2,
        /// <summary>플레이어 방어 능력(받는 피해 감소) 관련</summary>
        Strong=3,
        /// <summary>피해를 가하는 특성의 위력 관련</summary>
        Ability=4,
    }
    public int[] maxPoints { get; private set; } = new int[5] {30, 30, 30, 30, 30};
    public int[] points { get; private set; } = new int[5] {0, 0, 0, 0, 0};
    [SerializeField] Player player;

    private void Awake() {
        if(player == null) {
            GameObject pobj = GameObject.FindGameObjectWithTag("Player");
            if(pobj != null)
                player = pobj.GetComponent<Player>();
        }
    }
    private void Start() {
        if(GameManager.instance.gameData != null) {
            points = GameManager.instance.gameData.adaptation;
            int nextRemainingPoint = GameManager.instance.gameData.rate - 1 /**/+ 40/**/;
            foreach(int point in points) {
                nextRemainingPoint -= point;
            }
            remainingPoint = nextRemainingPoint;
        }
    }
    public void IncreasePoint() {
        remainingPoint ++;
    }
    /// <summary>Increase target Adaptation and return the value of result.</summary>
    public int Increase(Type targetType) {
        if(remainingPoint <= 0)
            throw new NotEnoughPointException();
        if(points[(int)targetType] >= maxPoints[(int)targetType])
            throw new MaxPointException();

        remainingPoint --;
        return IncreaseValue(targetType, 1);
    }
    public int Increase(Type targetType, int count) {
        if(remainingPoint <= 0)
            throw new NotEnoughPointException();
        if(points[(int)targetType] >= maxPoints[(int)targetType])
            throw new MaxPointException();
            
        int c = Mathf.Min(count, remainingPoint);
        c = Mathf.Min(c, maxPoints[(int)targetType] - points[(int)targetType]);

        remainingPoint -= c;
        return IncreaseValue(targetType, c);
    }
    /// <summary>Decrease target Adaptation and return the value of result.</summary>
    public int Decrease(Type targetType) {
        if(points[(int)targetType] <= 0)
            throw new MinPointException();

        remainingPoint ++;
        return IncreaseValue(targetType, -1);
    }
    public int Decrease(Type targetType, int count) {
        if(points[(int)targetType] <= 0)
            throw new MinPointException();

        int c = Mathf.Min(count, points[(int)targetType]);

        remainingPoint += c;
        return IncreaseValue(targetType, -c);
    }
    private int IncreaseValue(Type targetType, int amount) {
        int nextValue = 0;
        switch(targetType) {
            case Type.Power:
                points[(int)Type.Power] += amount;
                nextValue = points[(int)Type.Power];
                break;
            case Type.Movement:
                points[(int)Adaptation.Type.Movement] += amount;
                nextValue = points[(int)Adaptation.Type.Movement];
                break;
            case Type.Fast:
                points[(int)Adaptation.Type.Fast] += amount;
                nextValue = points[(int)Adaptation.Type.Fast];
                break;
            case Type.Strong:
                points[(int)Adaptation.Type.Strong] += amount;
                nextValue = points[(int)Adaptation.Type.Strong];
                break;
            case Type.Ability:
                points[(int)Adaptation.Type.Ability] += amount;
                nextValue = points[(int)Adaptation.Type.Ability];
                break;
            default:
                Debug.LogWarning("undefined adaptation type was referd.");
                break;
        }
        return nextValue;
    }
    class NotEnoughPointException : System.Exception {
        public NotEnoughPointException() : base("남은 적응 포인트가 부족합니다.") {}
    }
    class MaxPointException : System.Exception {
        public MaxPointException() : base("해당 적응 수치가 최대치에 도달하였습니다.") {}
    }
    class MinPointException : System.Exception {
        public MinPointException() : base("적응 수치는 0 미만으로 감소할 수 없습니다.") {}
    }
}

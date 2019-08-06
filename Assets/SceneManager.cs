using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class SceneManager : MonoBehaviour
{
    [Header("Drag objects into these lists")]
    private List<PickUps> _coins = new List<PickUps>();
    private List<PickUps> _badPallets = new List<PickUps>();
    [SerializeField] private FuseTrailScript _trail;
    //[SerializeField] private List<PickUps> _goodPallets = new List<PickUps>();
    // [SerializeField] private List<Barrel> _barrels = new List<Barrel>();

    [Header("Scene parameters")]
    [SerializeField] private int _amtCoinsNeeded = 5;


    private int _pickedUpCoinsAmt = 0;
    private int _pickedUpPallets = 0;

    public UnityEvent PickedUpAllCoins = new UnityEvent();
    public UnityEvent PickedUpABadPallet = new UnityEvent();



    private void Start()
    {
        //_coins.AddRange((IEnumerable<PickUps>)GameObject.FindGameObjectsWithTag("PickUp").Where(x => x.GetComponent<PickUps>().Bad));
        //_badPallets.AddRange((IEnumerable<PickUps>)GameObject.FindGameObjectsWithTag("Coin").Where(x => x.GetComponent<PickUps>().Bad));

        // Register all coins
        foreach (PickUps pu in _coins)
            pu.GotPickedUp.AddListener(CoinPickedUp);

        // Register all bad pallets
        foreach (PickUps pall in _badPallets)
            if (pall.score < 0) // just safety
                pall.GotPickedUp.AddListener(BadPalletPickedUp);

        //// Register all good pallets
        //foreach (PickUps pall in _goodPallets)
        //    if (pall.score > 0)
        //        pall.GotPickedUp.AddListener(GoodPalletPickedUp);
    }



    private void CoinPickedUp()
    {
        _pickedUpCoinsAmt++;
        if (_pickedUpCoinsAmt == _coins.Count)
        {
            PickedUpAllCoins.Invoke();
            GoToNext();
        }
    }
    private void BadPalletPickedUp()
    {
        PickedUpABadPallet.Invoke();
        ResetScene();
    }
    //private void GoodPalletPickedUp()
    //{
    //    _pickedUpPallets++;
    //}


    private void HandleFuseEnd()
    {
        if (_pickedUpCoinsAmt >= _amtCoinsNeeded)
        {
            GoToNext();
            return;
        }

        ResetScene();
    }

    private void ResetScene()
    {
        //TODO: Logic
    }
    private void GoToNext()
    {
        //TODO: Logic
    }
}

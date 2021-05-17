using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiscordVROverlay
{
    public class FavoritesBar : MonoBehaviour
    {
        [SerializeField]
        private GameObject favortiePrefab;
        [SerializeField]
        private int maxFavorites = 8;

        public static FavoritesBar instance;
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        public void AddFavorite(Server server, Channel channel)
        {
            FavoriteChannel fc = Instantiate(favortiePrefab, transform).GetComponent<FavoriteChannel>();
            fc.SetChannel(server, channel);
            fc.transform.SetSiblingIndex(0);
            if (transform.childCount > maxFavorites)
            {
                Destroy(transform.GetChild(maxFavorites-1).gameObject);
            }
        }
    }
}

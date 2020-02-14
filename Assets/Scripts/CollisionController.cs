using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionController : RaycastController
{

    public CollisionInfo collisions;

    LayerMask path;

    public void Move(Vector3 moveAmount, LayerMask path) //Velocity = direction du personnage, si input.y = -1 alors il passe au travers des plateformes
    {
        this.path = path;

        UpdateRaycastOrigins(); // on met à jour la position des rayons à chaque nouvelle position du personnage
        collisions.Reset(); // on reset les collisions pour les recalculer

        if (moveAmount.x != 0) //on évite les calculs de collision inutile en les calculant uniquement lorsque le personnage bouge
        {
            HorizontalCollisions(ref moveAmount);
        }

        if (moveAmount.y != 0) // même raisonnement pour l'axe y
        {
            VerticalCollisions(ref moveAmount);
        }
        
        transform.Translate(moveAmount);
        Physics2D.SyncTransforms();

    }

    void HorizontalCollisions(ref Vector3 velocity) // change la vélocité du personnage en fonction de ses collisions, et mets à jour collisionsInfo
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth; // les rayons regardent à une distance égale à celle traversé en une frame

        for (int i = 0; i < horizontalRayCount; i++) // on dessine les rayon un à un et vérifions s'il touche
        {
            Vector2 rayOrigins = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight; // si l'on va à gauche (directionX == -1) on lance les rayons de la gauche sinon la droite
            rayOrigins += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigins, Vector2.right * directionX, rayLength, path); // si le rayon touche un object de type collisionMask hit = true

            Debug.DrawRay(rayOrigins, Vector2.right * directionX, Color.red);

            if (hit)
            {
                velocity.x = (hit.distance - skinWidth) * directionX; // on réduit la vitesse en fonction de la distance qui nous sépare de l'obstacle
                rayLength = hit.distance; // on réajuste les rayons à la distance de l'obstacle

                collisions.left = directionX == -1; 
                collisions.right = directionX == 1; 
            }
        }
    }

    void VerticalCollisions(ref Vector3 velocity) 
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigins = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigins += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigins, Vector2.up * directionY, rayLength, path);
            Debug.DrawRay(rayOrigins, Vector2.up * directionY, Color.red);

            if (hit)
            {
                velocity.y = (hit.distance - skinWidth) * directionY; 
                rayLength = hit.distance;

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public void Reset()
        {
            above = below = false;
            left = right = false;
        }
    }
}

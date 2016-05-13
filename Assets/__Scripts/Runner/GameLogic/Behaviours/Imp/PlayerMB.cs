using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMB : MonoBehaviour, Interactuable {

    static PlayerMB instance;

    static public PlayerMB Instance{
        get { return instance; }
    }

    private Player player;
    public Player playerData{
        get { return player; }
        set { player = value; }
    }


    private ResourcesUni current_resource;
    public float update_ratio = 0.5f;
    private float current_time = 0;
    //private Animation current_anim;
    private eAnim current_anim;
    private int current_frame = 0;

    public ElementReference context;

    void Awake(){
        instance = this;
    }

    // Use this for initialization
    void Start () {
        updateResource ();
        if (current_resource != null) {
            string path = current_resource.getAssetPath (NPC.RESOURCE_TYPE_STAND_UP);

            //current_anim = Loader.loadAnimation(AssetsController.InputStreamCreatorEditor.getInputStreamCreator (path),path,new EditorImageLoader());
            current_anim = new eAnim (path);

            this.gameObject.name = player.getId ();

            current_frame = 0;

            // ## ANIMATION METHOD ##
            /*Texture2D tmp = current_anim.getFrame(0).getImage(false,false,0).texture;
               update_ratio = current_anim.getFrame(0).getTime();//Duration/1000f;*/

            // ## EANIM METHOD ##
            Texture2D tmp = current_anim.frames [0].Image;
            update_ratio = current_anim.frames [0].Duration / 1000f;
            this.GetComponent<Renderer> ().material.mainTexture = tmp;
            this.transform.localScale = new Vector3(tmp.width/10,tmp.height/10,1) * context.getScale();
        }


        Vector2 tmppos = new Vector2(context.getX(),context.getY()) / 10 + (new Vector2(0,-transform.localScale.y))/2;
        transform.localPosition = new Vector3(tmppos.x,60-tmppos.y,-context.getLayer());
    }

    public void setScale(float scale){
        Texture2D tmp = current_anim.frames[current_frame].Image;
        this.transform.localScale = new Vector3(tmp.width/10,tmp.height/10,1) * scale;
    }

    public Vector2 getPosition(){
        return new Vector2 (this.transform.localPosition.x, this.transform.localPosition.y - (this.current_anim.frames[current_frame].Image.height/10) / 2);
    }

    public float getHeight(){
        //return (current_anim.getFrame(current_frame).getImage(false,false,0).texture.height) * context.getScale();
        return (current_anim.frames[current_frame].Image.height) * context.getScale();
    }

    public void updateResource(){
        current_resource = null;
        foreach(ResourcesUni cr in player.getResources()){
            if (ConditionChecker.check(cr.getConditions())) {
                current_resource = cr;
                break;
            }
        }
    }

    public void changeFrame(){
        /*current_frame = (current_frame + 1) % current_anim.getFrames().Count;
        Texture2D tmp = current_anim.getFrame(current_frame).getImage(false,false,0).texture;
        update_ratio = current_anim.getFrame(current_frame).getTime();*/

        current_frame = (current_frame + 1) % current_anim.frames.Count;
        Texture2D tmp = current_anim.frames[current_frame].Image;
        update_ratio = current_anim.frames[current_frame].Duration/1000f;

        this.GetComponent<Renderer> ().material.mainTexture = tmp;
        this.transform.localScale = new Vector3(tmp.width/10,tmp.height/10,1) * context.getScale();
    }


    bool checkingTransparency = false;
    Vector3 start_pos, end_pos;
    float progress = 0.0f;
    float player_speed = 0.5f;
    float speed = 0f;

    void Update () {
        current_time += Time.deltaTime;

        if (current_time >= update_ratio){
            this.changeFrame();
            this.current_time = 0;
        }

        if (checkingTransparency) {
            RaycastHit hit;
            Physics.Raycast ( Camera.main.ScreenPointToRay (Input.mousePosition), out hit);

            if (current_anim.frames [current_frame].Image.GetPixelBilinear (hit.textureCoord.x, hit.textureCoord.y).a > 0f)
                showHand (true);
            else
                showHand (false);
        }

        if (moving && progress < 1.0f) {
            this.transform.localPosition = Vector3.Lerp (start_pos, end_pos, progress);
            Vector2 tmppos = getPosition ();
            this.context.setPosition ((int)(tmppos.x * 10), (int)((60 - tmppos.y) * 10));
            progress += Time.deltaTime * speed;
        } else if (moves.Count > 0) {
            move (moves.Dequeue ());
        } else if (moving && moves.Count <= 0) {
            this.current_anim = new eAnim (current_resource.getAssetPath (NPC.RESOURCE_TYPE_STAND_UP));
            moving = false;
        }
    }

    bool interactable = false;
    void showHand(bool show){
        if (show && !interactable) {
            Game.Instance.setCursor ("over");
            interactable = true;
        } else if (!show && interactable) {
            Game.Instance.setCursor ("default");
            interactable = false;
        }
    }

    void OnMouseEnter(){
        checkingTransparency = true;
    }

    void OnMouseExit() {
        checkingTransparency = false;
    }

    bool moving = false;
    Queue<KeyValuePair<Vector2,float>> moves = new Queue<KeyValuePair<Vector2,float>>();

    public void move(KeyValuePair<Vector2,float>[] points){
        moves = new Queue<KeyValuePair<Vector2,float>> (points);
    }
    public void moveInstant(Vector2 point){
        //Texture2D tmp = current_anim.frames[current_frame].Image;
        this.transform.localPosition = new Vector3 (point.x, point.y + this.transform.localScale.y/2,-context.getLayer());
    }

    void move(KeyValuePair<Vector2,float> point){
        moving = true;
        progress = 0.0f;
        this.start_pos = this.transform.localPosition;

        Texture2D tmp = current_anim.frames [current_frame].Image;
        this.end_pos = new Vector3(point.Key.x,point.Key.y + ((tmp.height/10f) * point.Value)/2,-context.getLayer());

        this.current_frame = 0;
        if (this.start_pos.x < this.end_pos.x) {
            this.current_anim = new eAnim (current_resource.getAssetPath (NPC.RESOURCE_TYPE_WALK_RIGHT));
        }else
            this.current_anim = new eAnim (current_resource.getAssetPath (NPC.RESOURCE_TYPE_WALK_LEFT));

        speed = player_speed * (50 / Vector2.Distance (start_pos, end_pos));
    }

    public InteractuableResult Interacted (RaycastHit hit = default(RaycastHit)){
        InteractuableResult res = InteractuableResult.IGNORES;

        return res;
    }


}

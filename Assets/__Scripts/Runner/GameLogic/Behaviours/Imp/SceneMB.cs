using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneMB : MonoBehaviour, Interactuable{

    public class MathFunction{
        Vector2 v1, v2;
        float z = 0;

        public MathFunction(Vector2 v1, Vector2 v2){
            this.v1 = v1;
            this.v2 = v2;
            this.z = (v2.y - v1.y) / (v2.x - v1.x);
        }

        public float getY(float x){
            return (z * x - z * v1.x + v1.y);
        }

        public float getX(float y){
            return ( (1/z) * y - (1/z) * v1.y + v1.x);
        }

        public Vector2[] contactPoints(Vector2 point){
            List<Vector2> ret = new List<Vector2> ();

            if(point.x <= Mathf.Max(v1.x,v2.x) && point.x >= Mathf.Min(v1.x,v2.x))
                ret.Add(new Vector2 (point.x, getY (point.x)));

            if(point.y <= Mathf.Max(v1.y,v2.y) && point.x >= Mathf.Min(v1.y,v2.y))
                ret.Add(new Vector2 (getX(point.y), point.y));

            return ret.ToArray ();
        }
    }

    public class LineHandler{
        /*public class LineHandlerComparer : IComparer<LineHandler> {
            public int Compare(LineHandler x, LineHandler y){
                return x.side.getLength () - y.side.getLength ();
            }
        }*/

        public const float DIVISOR = 10f;

        public Trajectory.Node start, end;
        public Trajectory.Side side;

        private float min_x, max_x, min_y, max_y;
        public MathFunction function;

        private MathFunction scale_function;

        private List<LineHandler> neighbours = new List<LineHandler>();

        public static Trajectory.Node commonPoint(LineHandler l1, LineHandler l2){
            if (l1.containsNode (l2.start))
                return l2.start;
            else
                return l2.end;
        }

        public static Vector2 nodeToVector2(Trajectory.Node node){
            return new Vector2 (node.getX () / LineHandler.DIVISOR, 60f - node.getY () / LineHandler.DIVISOR);
        }

        public LineHandler(Trajectory.Node start, Trajectory.Node end, Trajectory.Side side){
            this.start = start;
            this.end = end;
            this.side = side;

            float start_x = start.getX()/LineHandler.DIVISOR, end_x = end.getX()/LineHandler.DIVISOR
                , start_y = 60f - start.getY()/LineHandler.DIVISOR, end_y = 60f - end.getY()/LineHandler.DIVISOR;


            min_x = Mathf.Min(start_x, end_x);
            max_x = Mathf.Max(start_x, end_x);

            min_y = Mathf.Min(start_y, end_y);
            max_y = Mathf.Max(start_y, end_y);


            this.function = new MathFunction(new Vector2(start_x,start_y) ,new Vector2(end_x,end_y));

            this.scale_function = new MathFunction(new Vector2(start_x,start.getScale()), new Vector2(end_x,end.getScale()));
        }

        public Vector2[] contactPoints(Vector2 point){
            return function.contactPoints (point);
        }

        public bool contains(Vector2 v){
            bool ret = false;
            float x = function.getX (v.y), y = function.getY (v.x);


            if (
                        (v.x >= min_x && v.x <= max_x)
                    &&
                        (v.y >= min_y && v.y <= max_y)
                &&
                        (v.x >= (x - 0.01) && v.x <= (x + 0.01)) 
                    && 
                        (v.y >= (y - 0.01) && v.y <= (y + 0.01))
            )
                ret = true;
            
            return ret;
        }

        public bool containsNode(Trajectory.Node node){
            return (start == node) || (end == node);
        }

        public Trajectory.Node getOtherPoint(Trajectory.Node point){
            if (start == point)
                return end;
            else if(end == point)
                return start;

            return null;
        }

        public void addNeighbour(LineHandler n){
            this.neighbours.Add (n);
        }

        public bool isNeighbor(LineHandler line){
            return neighbours.Contains (line);
        }

        public LineHandler[] getNeighborLines(){
            return neighbours.ToArray ();
        }

        /*public LineHandler[] getNeighborLinesSorted(){
            List<LineHandler> n;
            n.Sort (new LineHandlerComparer());

            return n.ToArray ();
        }*/

        public List<Trajectory.Node> neighbor_nodes;
        public Trajectory.Node[] getNeighborNodes(){
            if (neighbor_nodes == null) {
                neighbor_nodes = new List<Trajectory.Node> ();
                foreach(LineHandler line in neighbours){
                    if (line.containsNode (this.start)) {
                        neighbor_nodes.Add (line.end);
                    } else
                        neighbor_nodes.Add (line.start);
                }
            }

            return neighbor_nodes.ToArray ();
        }

        public float getScaleFor(Vector2 point){
            return scale_function.getY (point.x);
        }
    }
	
	public GameObject Exit_Prefab;
	public GameObject ActiveArea_Prefab;
	public GameObject Character_Prefab;
	public GameObject Object_Prefab;
	public GameObject Atrezzo_Prefab;
    public GameObject Player_Prefab;
	
    private GeneralScene sd;

    public GeneralScene sceneData{
		get { return sd; }
		set { sd = value; }
	}

    private ElementReference player_context;
    private PlayerMB player;
    private List<LineHandler> lines = new List<LineHandler>();
    private Dictionary<ElementReference,ElementReference> contexts = new Dictionary<ElementReference, ElementReference>();
	
	// Use this for initialization
	void Start () {
        this.gameObject.name = sd.getId ();
		renderScene ();
	}

    Exit exit;
	
    bool fading = false;
    float total_time = 0f, current_time = 0f;

    void FixedUpdate () {
        if(fading){
            current_time -= Time.deltaTime;
            float alpha = current_time / total_time;
            colorChilds (new Color (1, 1, 1, alpha));
            if (alpha <= 0) {
                GameObject.Destroy (this.gameObject);
            }
        }

		if (movieplayer != MovieState.NOT_MOVIE && movieplayer != MovieState.PLAYING) {
            Interacted ();
        }
    }

    public void destroy(float time = 0){
        if (time != 0) {
            total_time = time;
            current_time = time;
            fading = true;
            this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y, this.transform.position.z - 10);
        }else
            GameObject.Destroy (this.gameObject);
    }

    private eAnim slides;
	private int current_slide;
	//private SlidesceneResource current_resource;

	public void renderScene(){
        ElementReference auxEleRef;
        switch (sd.getType()) {
        case GeneralScene.GeneralSceneSceneType.VIDEOSCENE:
            StartCoroutine (loadMovie ());
            this.transform.FindChild ("Background").localPosition = new Vector3 (40, 30, 20);
            break;
        case GeneralScene.GeneralSceneSceneType.SCENE: 
            Scene rsd = (Scene)sd;
            foreach (ResourcesUni sr in rsd.getResources()) {
                if (ConditionChecker.check (sr.getConditions ())) {
                    Texture2DHolder th = new Texture2DHolder (sr.getAssetPath (Scene.RESOURCE_TYPE_BACKGROUND));
                    Texture2D tmp = th.Texture;

                    Transform background = this.transform.FindChild ("Background");
                    background.GetComponent<Renderer> ().material.mainTexture = tmp;
                    float scale = (tmp.width / (tmp.height / 600f)) / 800f;

                    this.transform.position = new Vector3 (40, 30, 20);
                    background.localPosition = new Vector3 (((80 * scale) - 80) / 2f, 0, 20);
                    background.transform.localScale = new Vector3 (scale * 80, 60, 1);
                    break;
                }
            }
			
            Transform characters = this.transform.FindChild ("Characters");
            foreach (Transform child in characters) {
                GameObject.Destroy (child.gameObject);
            }
			
            foreach (ElementReference cr in rsd.getCharacterReferences()) {
                if (ConditionChecker.check (cr.getConditions ())) {
                    GameObject ret = GameObject.Instantiate (Character_Prefab);
                    Transform trans = ret.GetComponent<Transform> ();
                    ret.GetComponent<CharacterMB> ().context = cr;
                    ret.GetComponent<CharacterMB> ().charData = Game.Instance.getCharacter (cr.getTargetId ());
                    trans.SetParent (characters);
                }
            }
			
            Transform objects = this.transform.FindChild ("Objects");
            foreach (Transform child in objects) {
                GameObject.Destroy (child.gameObject);
            }
			
            List<ElementReference> items = rsd.getItemReferences ();

            ElementReference tmpElement;
            foreach (ElementReference ir in rsd.getItemReferences()) {
                if (ConditionChecker.check (ir.getConditions ())) {
                    if (!contexts.ContainsKey (ir)) {
                        tmpElement = new ElementReference (ir.getTargetId (), ir.getX (), ir.getY ());
                        tmpElement.setScale (ir.getScale ());
                        contexts.Add (ir, tmpElement);
                    }
                    
                    GameObject ret = GameObject.Instantiate (Object_Prefab);
                    Transform trans = ret.GetComponent<Transform> ();
                    ret.GetComponent<ObjectMB> ().context = contexts[ir];
                    ret.GetComponent<ObjectMB> ().objData = Game.Instance.getObject (ir.getTargetId ());
                    trans.SetParent (objects);
                }
            }
			
            Transform atrezzos = this.transform.FindChild ("Atrezzos");
            foreach (Transform child in atrezzos) {
                GameObject.Destroy (child.gameObject);
            }
			
            foreach (ElementReference ir in rsd.getAtrezzoReferences()) {
                if (ConditionChecker.check (ir.getConditions ())) {
                    GameObject ret = GameObject.Instantiate (Atrezzo_Prefab);
                    Transform trans = ret.GetComponent<Transform> ();
                    ret.GetComponent<AtrezzoMB> ().context = ir;
                    ret.GetComponent<AtrezzoMB> ().atrData = Game.Instance.getAtrezzo (ir.getTargetId ());
                    trans.SetParent (atrezzos);
                }
            }
			
            Transform activeareas = this.transform.FindChild ("ActiveAreas");
            foreach (Transform child in activeareas) {
                GameObject.Destroy (child.gameObject);
            }

            foreach (ActiveArea ad in rsd.getActiveAreas()) {
                if (ConditionChecker.check (ad.getConditions ())) {
                    GameObject ret = GameObject.Instantiate (ActiveArea_Prefab);
                    Transform trans = ret.GetComponent<Transform> ();
                    ret.GetComponent<ActiveAreaMB> ().aaData = ad;
                    trans.localScale = new Vector3 (ad.getWidth () / 10f, ad.getHeight () / 10f, 1);
                    Vector2 tmppos = new Vector2 (ad.getX (), ad.getY ()) / 10 + (new Vector2 (trans.localScale.x, trans.localScale.y)) / 2;
					
                    trans.localPosition = new Vector2 (tmppos.x, 60 - tmppos.y);
                    trans.SetParent (activeareas);
                }
            }

            Transform exits = this.transform.FindChild ("Exits");
            foreach (Transform child in exits) {
                GameObject.Destroy (child.gameObject);
            }
			
            foreach (Exit ed in rsd.getExits()) {
                if (ed.isHasNotEffects() || ConditionChecker.check (ed.getConditions ())) {
                    GameObject ret = GameObject.Instantiate (Exit_Prefab);
                    Transform trans = ret.GetComponent<Transform> ();
                    ret.GetComponent<ExitMB> ().exitData = ed;
                    trans.localScale = new Vector3 (ed.getWidth () / 10f, ed.getHeight () / 10f, 1);
                    Vector2 tmppos = new Vector2 (ed.getX (), ed.getY ()) / 10 + (new Vector2 (trans.localScale.x, trans.localScale.y)) / 2;
					
                    trans.localPosition = new Vector2 (tmppos.x, 60 - tmppos.y);
                    trans.SetParent (exits);
                }
            }

            if (!Game.Instance.isFirstPerson ()) {
                lines = new List<LineHandler> ();
                foreach (Trajectory.Side side in rsd.getTrajectory().getSides()) {
                    lines.Add (new LineHandler (rsd.getTrajectory ().getNodeForId (side.getIDStart ())
                        , rsd.getTrajectory ().getNodeForId (side.getIDEnd ())
                        , side));
                }
                updateNeighbours ();

                if (player_context == null) {
                    //Vector2 pos = LineHandler.nodeToVector2 (lines [lines.Count-1].end);
                    Trajectory.Node pos = lines [lines.Count-1].end;
                    player_context = new ElementReference ("Player", pos.getX(), pos.getY(), rsd.getPlayerLayer());
                    player_context.setScale (pos.getScale());
                }
                /*GameObject.Destroy(this.transform.FindChild ("Player"));*/

                player = GameObject.Instantiate (Player_Prefab).GetComponent<PlayerMB>();
                player.transform.parent = characters;
                player.playerData = Game.Instance.getPlayer ();
                player.context = player_context;
            }

			break;
        case GeneralScene.GeneralSceneSceneType.SLIDESCENE: 
            Slidescene ssd = (Slidescene) sd;
            foreach(ResourcesUni r in ssd.getResources()){
                if(ConditionChecker.check(r.getConditions())){
                    this.slides = new eAnim (r.getAssetPath (Slidescene.RESOURCE_TYPE_SLIDES));
                    this.transform.FindChild("Background").GetComponent<Renderer> ().material.mainTexture = this.slides.frames[0].Image;
					this.transform.position = new Vector3 (40, 30, 20);
					break;
				}
			}
			break;
		}
	}

    private void updateNeighbours(){
        foreach (LineHandler line1 in lines) {
            foreach (LineHandler line2 in lines) {
                if (line1 != line2 && !line1.isNeighbor (line2) && (line1.containsNode(line2.start) || line1.containsNode(line2.end)) ) {
                    line1.addNeighbour (line2);
                    line2.addNeighbour (line1);
                }
            }
        }
    }

    public Vector2 closestPoint(Vector2 v){
        Vector2 ret = new Vector2(0f,0f);

        float distance = float.MaxValue, current = float.MaxValue;

        LineHandler tmp = lines[0];

        foreach (LineHandler handler in lines) {
            foreach (Vector2 collisions in handler.contactPoints(v)) {
                current = Vector2.Distance (v, collisions);
                if (current < distance) {
                    distance = current;
                    ret = collisions;
                    tmp = handler;
                }
            }
        }

        bool contains = tmp.contains (ret);

        return ret;
    }

    public KeyValuePair<Vector2,float>[] route(Vector2 origin, Vector2 destiny){
        List<KeyValuePair<Vector2,float>> ret = new List<KeyValuePair<Vector2,float>> ();

        LineHandler origin_line = null, destiny_line = null;
        foreach (LineHandler handler in lines) {
            if (origin_line == null && handler.contains (origin))
                origin_line = handler;

            if (destiny_line == null && handler.contains (destiny))
                destiny_line = handler;

            if (origin_line != null && destiny_line != null)
                break;
        }

        Vector2 closest = Vector2.zero;
        if (origin_line == null) {
            closest = closestPoint (PlayerMB.Instance.getPosition());
            foreach (LineHandler handler in lines) {
                if (origin_line == null && handler.contains (closest)) {
                    origin_line = handler;
                    break;
                }
            }
        }

        if (closest != Vector2.zero)
            ret.Add (new KeyValuePair<Vector2,float>(closest,origin_line.getScaleFor(closest)));

        if (origin_line != null && destiny_line != null) {
            //######################################################
            // IF ORIGIN_LINE AND DESTINY_LINE ARE THE SAME
            // Return only the destiny point, dont have to go
            // to other node
            //######################################################
            if (origin_line == destiny_line) {
                ret.Add (new KeyValuePair<Vector2,float>(destiny,destiny_line.getScaleFor(destiny)));
            } else {
                List<KeyValuePair<Vector2,float>> tmpRoute = new List<KeyValuePair<Vector2, float>>(reach (origin_line, destiny_line));
                //tmpRoute.Reverse ();
                ret.AddRange(tmpRoute);
                ret.Add (new KeyValuePair<Vector2,float>(destiny,destiny_line.getScaleFor(destiny)));
            }
        }

        return ret.ToArray ();
    }

    public KeyValuePair<Vector2,float>[] reach(LineHandler origin, LineHandler destiny){
        List<KeyValuePair<Vector2,float>> route = new List<KeyValuePair<Vector2,float>> ();
        //Scene s = (Scene) sd;

        //WE STORE IN A BOOLEAN IF WE HAVE VISITED OR NOT THAT LINE
//        Dictionary<LineHandler, bool> visited_lines = new Dictionary<LineHandler, bool>();
//        Dictionary<Trajectory.Node, bool> visited_nodes = new Dictionary<Trajectory.Node, bool>();
//
//        foreach (LineHandler line in lines)
//            visited_lines.Add (line, false);
//
//        reach (origin, destiny, visited_lines, route);

        Dictionary<LineHandler, KeyValuePair<float,LineHandler>> stickered = stick (origin);

        LineHandler current = destiny;
        while (current != origin) {
            KeyValuePair<float,LineHandler> node = stickered [current];
            Trajectory.Node tmp = LineHandler.commonPoint (current, node.Value);
            route.Add (new KeyValuePair<Vector2, float>(LineHandler.nodeToVector2 (tmp),tmp.getScale()));
            current = node.Value;
        }

        route.Reverse ();

        return route.ToArray ();
    }

    public Dictionary<LineHandler, KeyValuePair<float,LineHandler>> stick(LineHandler origin){
        Dictionary<LineHandler, KeyValuePair<float,LineHandler>> stickered = new Dictionary<LineHandler, KeyValuePair<float, LineHandler>>();
        Dictionary<LineHandler,float> costs = new Dictionary<LineHandler, float>();
        LineHandler current = origin, previous = origin;

        stickered.Add (origin, new KeyValuePair<float, LineHandler> (0, null));

        float current_cost = 0, total_cost = 0;
        while (stickered.Count < lines.Count) {
            current_cost = current.side.getLength ();
            total_cost = stickered [current].Key;
            foreach (LineHandler line in current.getNeighborLines ()) {
                if(!stickered.ContainsKey(line))
                    if (costs.ContainsKey (line)) {
                        if (costs [line] > current_cost) {
                            costs [line] = current_cost;
                        }
                    } else {
                        costs.Add (line, current_cost);
                    }
            }

            //obtenemos la mas corta
            float min = float.MaxValue;
            LineHandler selected = origin;
            foreach (KeyValuePair<LineHandler,float> line in costs) {
                if (line.Value < min) {
                    selected = line.Key;
                    min = line.Value;
                }
            }

            costs.Remove(selected);

            //establecemos la mas corta como principal y la añadimos a vecinas
            stickered.Add(selected, new KeyValuePair<float, LineHandler>(stickered[previous].Key + current.side.getLength(),current));
            previous = current;
            current = selected;
        }

        return stickered;
    }

    /*public bool reach(Trajectory.Node origin, LineHandler origin_line, LineHandler destiny, Dictionary<Trajectory.Node, bool> visited_nodes,  Dictionary<LineHandler, bool> visited_lines, List<Vector2> route){
        bool reached = true;
        if(visited_nodes[origin] == false && !destiny.containsNode(origin)){
            visited_nodes [origin] = true;
            foreach (LineHandler line in origin_line.getNeighborLines()) {
                if(visited_lines[line] == false && line.containsNode(origin)){
                    visited_lines[line.Key] = true;
                    if (!visited_nodes[line.Key.start] && reach (line.Key.start, destiny, visited_nodes, visited_lines, route))
                        route.Add (new Vector2(line.Key.start.getX () / 10f, 60 - line.Key.start.getY () / 10f));
                    else if (!visited_nodes[line.Key.end] && reach (line.Key.end, destiny, visited_nodes, visited_lines, route))
                        route.Add (new Vector2(line.Key.end.getX () / 10f, 60 - line.Key.end.getY () / 10f));
                    else
                        reached = false;
                }
            }
        }
        return reached;
    }*/

    /*public bool reach(Trajectory.Node origin, LineHandler origin_line, LineHandler destiny, Dictionary<Trajectory.Node, bool> visited_nodes,  Dictionary<LineHandler, bool> visited_lines, List<Vector2> route){
        bool reached = true;
        if (!destiny.containsNode (origin)) {
            visited_nodes [origin] = true;
            foreach (LineHandler line in origin_line.getNeighborLines()) {
                if (!visited_lines [line] && !visited_nodes [line.getOtherPoint (origin)] && reach (line.getOtherPoint (origin), origin_line, destiny, visited_nodes, visited_lines, route)) {
                    Trajectory.Node n = line.getOtherPoint (origin);
                    route.Add (new Vector2(n.getX () / 10f, 60 - n.getY () / 10f));
                }
            }
        }
        return reached;
    }*/

    public bool reach(LineHandler origin, LineHandler destiny, Dictionary<LineHandler, bool> visited, List<KeyValuePair<Vector2,float>> route){
        bool ret = false;

        if (origin == destiny)
            ret = true;
    else if (visited[origin])
            ret = false;
        else{
            visited [origin] = true;
            foreach (LineHandler neighbor in origin.getNeighborLines()) {
                if (reach (neighbor, destiny, visited, route)) {
                    Trajectory.Node point = LineHandler.commonPoint (origin, neighbor);
                    route.Add (new KeyValuePair<Vector2,float>(LineHandler.nodeToVector2 (point),point.getScale()));
                    ret = true;
                    break;
                }
            }
        }

        return ret;

    }


	
    public InteractuableResult Interacted (RaycastHit hit = default(RaycastHit)){
		bool forcewait = false;
        Effects e;
		switch (sceneData.getType ()) {
        case GeneralScene.GeneralSceneSceneType.SCENE:
            if(!Game.Instance.isFirstPerson())
                PlayerMB.Instance.move(route (PlayerMB.Instance.getPosition(), closestPoint (Camera.main.ScreenToWorldPoint (Input.mousePosition))));
            break;
        case GeneralScene.GeneralSceneSceneType.SLIDESCENE:
            if(current_slide+1 < this.slides.frames.Count){
				current_slide++;
                this.transform.FindChild("Background").GetComponent<Renderer> ().material.mainTexture = this.slides.frames[current_slide].Image;
				forcewait = true;
			}else{
                switch(((Slidescene)sceneData).getNext ()){
                    case Slidescene.NEWSCENE:
                        Game.Instance.renderScene (((Slidescene)sceneData).getTargetId()); 
                    break;
                case Slidescene.ENDCHAPTER: 
                    //Game.Instance(); 
                    break;
				}

                e = ((Slidescene)sceneData).getEffects ();
                if(e!= null)
                    Game.Instance.Execute(new EffectHolder(e));
				
			}
			break;
        case GeneralScene.GeneralSceneSceneType.VIDEOSCENE:
            switch(((Videoscene)sceneData).getNext ()){
                case Slidescene.NEWSCENE:
                    Game.Instance.renderScene (((Videoscene)sceneData).getTargetId());
                    break;
                case Slidescene.ENDCHAPTER: 
                    break;
            }

            e = ((Videoscene)sceneData).getEffects ();
            if(e!= null)
                Game.Instance.Execute(new EffectHolder(e));
            break;
		}

        return forcewait ? InteractuableResult.REQUIRES_MORE_INTERACTION : InteractuableResult.IGNORES ;
	}

    private void colorChilds(Color color){
        foreach (Transform t1 in transform) {
            if (t1.name != "Exits") {
                if (t1.GetComponent<Renderer> () != null)
                    t1.GetComponent<Renderer> ().material.color = color;
            
                foreach (Transform t2 in t1) {
                    if (t2.GetComponent<Renderer> () != null)
                        t2.GetComponent<Renderer> ().material.color = color;
                }
            }
        }
    }

    // VIDEOSCENE FUNCTIONS
private enum MovieState { NOT_MOVIE, LOADING, PLAYING, STOPPED };
private MovieState movieplayer;

#if UNITY_ANDROID
	//Handheld.PlayFullScreenMovie("");
	IEnumerator loadMovie(){
	yield break;
	}
#else
	MovieTexture movie;
	string url_prefix = "file:///";
	IEnumerator loadMovie(){
		string videoname = ((Videoscene)sceneData).getResources () [0].getAssetPath (Videoscene.RESOURCE_TYPE_VIDEO);
		string dir = "";
		if (System.IO.File.Exists (Game.Instance.getSelectedGame() + videoname.Split ('.') [0] + ".ogv"))
		dir = url_prefix + Game.Instance.getSelectedGame() + videoname.Split ('.') [0] + ".ogv";
		else
		dir = url_prefix + Game.Instance.getSelectedGame() + videoname;

		WWW www = new WWW (dir);
		yield return www;

		if (www.error != null) {
			Debug.Log ("Error: Can't laod movie! - " + www.error);
			yield break;

		} else {
			MovieTexture video = www.movie as MovieTexture;
			Debug.Log("Movie loaded");
			Debug.Log(www.movie);
			movie = video;
			setMovie();
			playMovie();
		}
	}


	public void setMovie(){
		Debug.Log (movie.name + " ------");
		this.transform.FindChild ("Background").GetComponent<Renderer>().material.mainTexture = movie;
		//sound.clip = movie.audioClip;
	}

	public void playMovie(){
		movie.Play ();
		//sound.Play ();
	}

	public void pauseMovie(){
		movie.Pause ();
		//sound.Pause ();
	}

#endif
}

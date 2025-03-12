using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace RoundHero
{
    
	public class TextOutline : MonoBehaviour {

		public float pixelSize = 1;
		public Color outlineColor = Color.black;
		public bool resolutionDependant = false;
		public int doubleResolution = 1024;
		RectTransform rectTransform;
		[SerializeField]
		private TextMesh textMesh;
		private Color originalColor;

		void Start() {
			textMesh = GetComponent<TextMesh>();    
			rectTransform = this.GetComponent<RectTransform>();

			originalColor = textMesh.color;

			for (int i = 0; i < 8; i++) {
				GameObject outline = new GameObject("outline", typeof(TextMesh));
				outline.transform.parent = transform;
				outline.transform.localScale = new Vector3(1, 1, 1);
				RectTransform rectTransformChild = outline.GetComponent<RectTransform>();

				rectTransformChild.anchoredPosition = rectTransform.anchoredPosition;
				rectTransformChild.anchoredPosition3D = rectTransform.anchoredPosition3D;
				rectTransformChild.anchorMax = rectTransform.anchorMax;
				rectTransformChild.anchorMin = rectTransform.anchorMin;
				rectTransformChild.offsetMax = rectTransform.offsetMax;
				rectTransformChild.offsetMin = rectTransform.offsetMin;
				rectTransformChild.pivot = rectTransform.pivot;
				rectTransformChild.sizeDelta = rectTransform.sizeDelta;
			}

			Reposition();
		}

		void Reposition() {
			Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.position);

			outlineColor.a = textMesh.color.a * textMesh.color.a;
			textMesh.color = outlineColor;
			// copy attributes
			for (int i = 0; i < transform.childCount; i++) {

				TextMesh other = transform.GetChild(i).GetComponent<TextMesh>();
				other.color = outlineColor;
				other.text = textMesh.text;
				other.alignment = textMesh.alignment;
				other.font = textMesh.font;
				other.fontSize = textMesh.fontSize;
				other.fontStyle = textMesh.fontStyle;
				other.lineSpacing = textMesh.lineSpacing;

				bool doublePixel = resolutionDependant && (Screen.width > doubleResolution || Screen.height > doubleResolution);
				Vector3 pixelOffset = GetOffset(i) * (doublePixel ? 2.0f * pixelSize : pixelSize);
				Vector3 worldPoint = Camera.main.ScreenToWorldPoint(screenPoint + pixelOffset);
				other.transform.position = worldPoint;

				if(i == transform.childCount-1)
				{
					other.color = originalColor;
				}
			}
		}

		Vector3 GetOffset(int i) {
			switch (i % 8) {
			case 0: return new Vector3(0, 1, 0);
			case 1: return new Vector3(1, 1, 0);
			case 2: return new Vector3(1, 0, 0);
			case 3: return new Vector3(1, -1, 0);
			case 4: return new Vector3(0, -1, 0);
			case 5: return new Vector3(-1, -1, 0);
			case 6: return new Vector3(-1, 0, 0);
			case 7: return new Vector3(-1, 1, 0);
			default: return Vector3.zero;
			}
		}
	}

}
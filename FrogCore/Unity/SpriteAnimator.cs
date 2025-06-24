using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace FrogCore.Unity;

public class SpriteAnimator : MonoBehaviour
{
    public SpriteAnimation[] animations = new SpriteAnimation[0];
    public SpriteAnimation startAnimation;
    public bool playOnEnable = true;

    public bool playing {get; set;} = false;
    public SpriteAnimation current {get; set;}
    public int currentFrame {get; set;} = 0;
    public bool reversing {get; set;} = false;

    private SpriteRenderer _sprite;
    private float clock = 0f;


    void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        if (playOnEnable)
            Play(0);
    }

    void Update()
    {
        if (playing)
        {
            int moveFrames = 0;
            while (clock >= 1/current.fps)
            {
                clock -= 1/current.fps;
                //currentFrame++;
                if (reversing)
                    moveFrames--;
                else
                    moveFrames++;
            }
            clock += Time.deltaTime;
            if (currentFrame + moveFrames >= current.frames.Length && !reversing)
            {
                switch (current.loopType)
                {
                    case SpriteAnimationLoopType.Once:
                        Stop();
                        break;
                    case SpriteAnimationLoopType.Loop:
                        currentFrame += moveFrames - (current.frames.Length - current.loopStart);
                        break;
                    case SpriteAnimationLoopType.PingPong:
                        reversing = true;
                        currentFrame = current.frames.Length - (currentFrame + moveFrames - current.frames.Length) - 1;
                        break;
                }
            }
            else if (currentFrame + moveFrames < 0 && reversing)
            {
                switch (current.loopType)
                {
                    case SpriteAnimationLoopType.Once:
                        Stop();
                        break;
                    case SpriteAnimationLoopType.Loop:
                        currentFrame += moveFrames + current.frames.Length;
                        break;
                    case SpriteAnimationLoopType.PingPong:
                        reversing = false;
                        currentFrame = - (currentFrame + moveFrames);
                        break;
                }
            }
            else
                currentFrame += moveFrames;
        }
        currentFrame %= current.frames.Length;
        SetSprite(currentFrame);
    }

    public void Play(int frame = 0)
    {
        if (current)
            Play(current, frame);
        else if (startAnimation)
            Play(startAnimation, frame);
        else
            Play(0, frame);
    }

    public void Play(string name, int frame = 0, bool reversing = false) => Play(animations.First(anim => anim.name == name), frame, reversing);

    public void Play(int index, int frame = 0, bool reversing = false) => Play(animations[index], frame, reversing);

    public void Play(SpriteAnimation anim, int frame = 0, bool reversing = false)
    {
        current = anim;
        this.reversing = reversing;
        if (anim)
        {
            Play();
            currentFrame = frame;
            SetSprite(frame);
        }
        else
        {
            Stop();
            currentFrame = 0;
        }
        clock = 0;
    }

    public void Play()
    {
        if (!current)
            current = startAnimation;
        playing = true;
    }

    public void Stop() => playing = false;

    private void SetSprite(Sprite sprite) 
    {
        _sprite.sprite = sprite;
    }

    private void SetSprite(int frame) => SetSprite(current.frames[frame]);

    public IEnumerator PlayAnimWait(string name, int frame = 0, bool reversing = false) => PlayAnimWait(animations.First(anim2 => anim2.name == name), frame, reversing);

    public IEnumerator PlayAnimWait(int index, int frame = 0, bool reversing = false) => PlayAnimWait(animations[index], frame, reversing);
    
    public IEnumerator PlayAnimWait(SpriteAnimation anim, int frame = 0, bool reversing = false)
    {
        Play(anim, frame, reversing);
        yield return new WaitForSeconds((float)(anim.frames.Length - frame) / anim.fps);
    }
}

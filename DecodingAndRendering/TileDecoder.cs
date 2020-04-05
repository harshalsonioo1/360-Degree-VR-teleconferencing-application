 /*
* This function is used to decode each small tile and output the corresponding YUV format frame.
* Input: Filename of the corresponding file name of the small tile and the corresponding index number of this tile.
*/

public unsafe void Decoder(string fileName, int tilenumber)
    {

        int error, frame_count = 0;
        int got_picture, ret;
        SwsContext* pSwsCtx = null;
        AVFormatContext* ofmt_ctx = null;
        IntPtr convertedFrameBufferPtr = IntPtr.Zero;
        show_flag = 1;
        try
        {
            

            ffmpeg.avcodec_register_all();
            
           
            ofmt_ctx = ffmpeg.avformat_alloc_context();


            
            error = ffmpeg.avformat_open_input(&ofmt_ctx, fileName, null, null);
            if (error != 0)
            {
                throw new ApplicationException(ffmpeg.FFmpegBinariesHelper.GetErrorMessage(error));
                Debug.Log(fileName);
                Debug.Log("Log");
                show_flag = 1;
                return 0;
            }

           
            for (int i = 0; i < ofmt_ctx->nb_streams; i++)
            {
                if (ofmt_ctx->streams[i]->codec->codec_type == AVMediaType.AVMEDIA_TYPE_VIDEO)
                {
                    videoindex = i;
                    Debug.Log("video.............." + videoindex);
                }
            }

            if (videoindex == -1)
            {
                Debug.Log("Couldn't find a video stream");
                return -1;
            }

           
            if (videoindex > -1)
            {
                
                AVCodecContext* pCodecCtx = ofmt_ctx->streams[videoindex]->codec;

                
                AVCodec* pCodec = ffmpeg.avcodec_find_decoder(pCodecCtx->codec_id);
                if (pCodec == null)
                {
                    
                    show_flag = 2;
                    return -1;
                }

               
                if (ffmpeg.avcodec_open2(pCodecCtx, pCodec, null) < 0)
                {
                    
                    show_flag = 3;
                    return -1;
                }

                Debug.Log("Find a  video stream.channel=" + videoindex);

                
                var format = ofmt_ctx->iformat->name->ToString();
                var len = (ofmt_ctx->duration) / 1000000;
                var width = pCodecCtx->width;
                var height = pCodecCtx->height;
                pCodecCtx->thread_count = 8;
                show_flag = 0;
                
                AVPacket* packet = (AVPacket*)ffmpeg.av_malloc((ulong)sizeof(AVPacket));

                
                AVFrame* pFrame = ffmpeg.av_frame_alloc();
                //YUV420
                AVFrame* pFrameYUV = ffmpeg.av_frame_alloc();
                
                int out_buffer_size = ffmpeg.avpicture_get_size(AVPixelFormat.AV_PIX_FMT_YUV420P, pCodecCtx->width, pCodecCtx->height);
                byte* out_buffer = (byte*)ffmpeg.av_malloc((ulong)out_buffer_size);
               
                ffmpeg.avpicture_fill((AVPicture*)pFrameYUV, out_buffer, AVPixelFormat.AV_PIX_FMT_YUV420P, pCodecCtx->width, pCodecCtx->height);

                
                SwsContext* sws_ctx = ffmpeg.sws_getContext(pCodecCtx->width, pCodecCtx->height, AVPixelFormat.AV_PIX_FMT_YUV420P /*pCodecCtx->pix_fmt*/, pCodecCtx->width, pCodecCtx->height, AVPixelFormat.AV_PIX_FMT_YUV420P, ffmpeg.SWS_BICUBIC, null, null, null);
                int flag = 0;
                int skipped_frame = 0;

                while (ffmpeg.av_read_frame(ofmt_ctx, packet) >= 0)
                {
                    if (flag == 0)
                    {
                        semaphore[tilenumber].WaitOne();
                    }

                    
                    if (packet->stream_index == videoindex)
                    {
                       
                        ret = ffmpeg.avcodec_decode_video2(pCodecCtx, pFrame, &got_picture, packet);
                        if (ret < 0)
                        {
                            Debug.Log("视频解码错误");
                            return -1;
                        }

                        Debug.Log("当前跳过帧数：" + skipped_frame.ToString());
                        
                        if (got_picture > 0)
                        {
                            frame_count++;
                            Debug.Log("视频帧数:第 " + frame_count + " 帧");
                            //AVFrame
                            ffmpeg.sws_scale(sws_ctx, pFrame->data, pFrame->linesize, 0, pCodecCtx->height, pFrameYUV->data, pFrameYUV->linesize);                 
                            flag = 0;
                            cur[tilenumber] = pFrameYUV;
                            isTrue[tilenumber] = false; 
                        }
                        else
                        {
                           skipped_frame++;
                           flag = 1;
                        }
                        ffmpeg.av_free_packet(packet);
                    }
                    else
                    {
                        flag = 1;
                    }
  

                }
                
                flag = 0;
                for(int i = skipped_frame; i>0; i--)
                {
                    frame_count++;
                    if (flag == 0)
                    {
                        semaphore[tilenumber].WaitOne();
                    }
                    ret = ffmpeg.avcodec_decode_video2(pCodecCtx, pFrame, &got_picture, packet);
                    if (got_picture > 0)
                    {
                        
                        Debug.Log("视频帧数:第 " + frame_count + " 帧");
                        //AVFrame
                        ffmpeg.sws_scale(sws_ctx, pFrame->data, pFrame->linesize, 0, pCodecCtx->height, pFrameYUV->data, pFrameYUV->linesize);            
                        flag = 0;
                        cur[tilenumber] = pFrameYUV;
                        isTrue[tilenumber] = false; //
                    else
                    {
                        flag = 1;
                    }
                }
                Thread.Sleep(10);
                if (frame_count >= changeFrame)
                {
                    semaphore[tilenumber].Release();
                    ffmpeg.av_free_packet(packet);
                    ffmpeg.avformat_close_input(&ofmt_ctx);
                    ffmpeg.av_frame_free(&pFrameYUV);
                    ffmpeg.av_frame_free(&pFrame);
                    ffmpeg.avcodec_close(pCodecCtx);
                    ffmpeg.av_free(out_buffer);
                    Debug.Log("当前视频解码完毕" + tilenumber.ToString());
                    return 0;
                }

            }

        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            Thread.ResetAbort();
        }
        finally
        {
            if (&ofmt_ctx != null)
            {
                ffmpeg.avformat_close_input(&ofmt_ctx);
            }

        }
        IsRun = false;
        return 0;


    }
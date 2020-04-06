function [result]=CalPMSEPerTileGra(img_raw,img_target,R,T) 
    D_JND=double(abs(double(img_target)-double(img_raw)));
    C=D_JND-R;
    temp=(C>0).*(T==1).*(C.^2);
    temp_PSPNR=sum(temp(:)); 
    result=temp_PSPNR;
    D_JND=double(abs(img_target-img_raw));
    GD_JND=gpuArray(D_JND);
    GR=gpuArray(R);
    GT=gpuArray(T);
    GC=GD_JND-GR;
    temp=(GC>0).*(GT==1).*(GC.^2);
    temp_PSPNR=sum(temp(:));
    result=gather(temp_PSPNR);
end